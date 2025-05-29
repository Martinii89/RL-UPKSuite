using System.Collections.Concurrent;

using LazyCache;

using Microsoft.Extensions.FileSystemGlobbing;

using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types;

namespace RlUpk.Core.Utility;

/// <summary>
///     Configuration object for the <see cref="PackageCache" />
/// </summary>
public class PackageCacheOptions
{
    /// <summary>
    ///     The serializer is required. The other members are optional
    /// </summary>
    /// <param name="unrealPackageSerializer"></param>
    /// <param name="nativeClassFactory"></param>
    public PackageCacheOptions(IStreamSerializer<UnrealPackage> unrealPackageSerializer,
        INativeClassFactory nativeClassFactory)
    {
        UnrealPackageSerializer = unrealPackageSerializer;
        NativeClassFactory = nativeClassFactory;
    }

    /// <summary>
    ///     a list of package that the cache should refuse to load
    /// </summary>
    public List<string> PackageBlacklist { get; set; } = new();

    /// <summary>
    ///     The paths to search for related packages. Will default to cwd if no path is specified
    /// </summary>
    public List<string> SearchPaths { get; init; } = new();

    /// <summary>
    ///     The file extensions to search for. Do not include the dot, just the extension
    /// </summary>
    public List<string> Extensions { get; init; } = new()
    {
        "u", "upk"
    };

    /// <summary>
    ///     If the loader should auto link all packages or not
    /// </summary>
    public bool GraphLinkPackages { get; set; } = true;

    /// <summary>
    ///     The serializer to use when loading packages
    /// </summary>
    public IStreamSerializer<UnrealPackage> UnrealPackageSerializer { get; init; }

    /// <summary>
    ///     Optional. Use a package unpacker to enable auto-loading of packed\compressed packages
    /// </summary>
    public IPackageUnpacker PackageUnpacker { get; set; } = new NeverUnpackUnpacker();


    /// <summary>
    ///     Factory used to create serializers for all UObjects in the package
    /// </summary>
    public IObjectSerializerFactory? ObjectSerializerFactory { get; set; }

    /// <summary>
    ///     A object used to create the UClass objects for the native only classes
    /// </summary>
    public INativeClassFactory NativeClassFactory { get; set; }
}

/// <summary>
///     The PackageCache can find and cache packages for reuse. Especially useful when resolving resolving import object
///     (hence the name)
/// </summary>
public class PackageCache : IPackageCache
{
    private readonly ConcurrentDictionary<string, UnrealPackage> _cachedPackages =
        new(StringComparer.CurrentCultureIgnoreCase);

    private readonly PackageCacheOptions _options;

    private readonly Dictionary<string, string> _fileSearchCache = new(StringComparer.OrdinalIgnoreCase);
    
    IAppCache cache = new CachingService();

    /// <summary>
    ///     Constructs a configured PackageCache
    /// </summary>
    /// <param name="options"></param>
    public PackageCache(PackageCacheOptions options)
    {
        _options = options;
        InitFileSearchCache();
    }

    private void InitFileSearchCache()
    {
        Matcher matcher = new();
        matcher.AddIncludePatterns(_options.Extensions.Select(ext => $"*.{ext}"));
        var matchedFiles = new List<string>();
        foreach (var searchPath in _options.SearchPaths)
        {
            matchedFiles.AddRange(matcher.GetResultsInFullPath(searchPath));
        }

        foreach (string file in matchedFiles)
        {
            _fileSearchCache[Path.GetFileNameWithoutExtension(file)] = file;
        }
    }

    /// <summary>
    ///     Returns a cached package if previously loaded. If not it will search for the package and return it if found.
    /// </summary>
    /// <param name="packageName"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public UnrealPackage? ResolveExportPackage(string packageName)
    {
        if (_cachedPackages.TryGetValue(packageName, out var cachedPackage))
        {
            return cachedPackage;
        }

        if (_options.PackageBlacklist.Contains(packageName))
        {
            return null;
        }


        if (!_fileSearchCache.TryGetValue(packageName, out var packagePath))
        {
            return null;
        }

        Console.WriteLine($"[PackageCache]: Reading package {packageName}");

        var packageBuffer = cache.GetOrAdd(packagePath, () => GetBufferForPackage(packagePath));
        var packageStream = new MemoryStream(packageBuffer);

        var loadOptions = new UnrealPackageOptions(_options.UnrealPackageSerializer, packageName,
            _options.NativeClassFactory, this,
            _options.ObjectSerializerFactory);
        var unrealPackage = UnrealPackage.DeserializeAndInitialize(packageStream, loadOptions);

        // Add to cache before linking to avoid infinite recursive loop
        var added = _cachedPackages.TryAdd(packageName, unrealPackage);
        if (!added)
        {
            // Some other thread must have already added the package while this thread were parsing it. Return the object in the dictionary and discard whatever we created here
            Console.WriteLine($"[PackageCache]: Concurrent interference, reusing cached package {packageName}");
            return _cachedPackages[packageName];
        }

        Console.WriteLine($"[PackageCache]: Caching package {packageName}");


        if (_options.GraphLinkPackages)
        {
            unrealPackage.GraphLink();
            cache.Remove(packagePath);
        }


        return unrealPackage;
    }
    

    private byte[] GetBufferForPackage(string path)
    {
        Console.WriteLine($"[PackageCache]: Reading bytes for package {path}");
        // byte[] fileBytes = File.ReadAllBytes(path);
        // var packageStream = new MemoryStream(fileBytes);
        using var fileStream = File.OpenRead(path);
        if (!_options.PackageUnpacker.IsPackagePacked(fileStream))
        {
            return File.ReadAllBytes(path);
        }

        var unpackedStream = new MemoryStream();
        _options.PackageUnpacker.Unpack(fileStream, unpackedStream);
        unpackedStream.Position = 0;
        byte[] bufferForPackage = unpackedStream.GetBuffer();
        return bufferForPackage;

    }

    /// <inheritdoc />
    public UnrealPackage GetCachedPackage(string packageName)
    {
        return _cachedPackages[packageName];
    }

    /// <inheritdoc />
    public void AddPackage(UnrealPackage package)
    {
        _cachedPackages.TryAdd(package.PackageName, package);
    }

    /// <inheritdoc />
    public List<string> GetCachedPackageNames()
    {
        return _cachedPackages.Keys.ToList();
    }

    /// <inheritdoc />
    public bool IsPackageCached(string packageName)
    {
        return _cachedPackages.ContainsKey(packageName);
    }

    public void RemoveCachedPackage(UnrealPackage package)
    {
        _cachedPackages.Remove(package.PackageName, out var removedPackage);
    }
}