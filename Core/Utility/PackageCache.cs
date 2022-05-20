using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Types;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Core.Utility;

/// <summary>
///     Configuration object for the <see cref="PackageCache" />
/// </summary>
public class PackageCacheOptions
{
    /// <summary>
    ///     The serializer is required. The other members are optional
    /// </summary>
    /// <param name="unrealPackageSerializerFor"></param>
    public PackageCacheOptions(IStreamSerializerFor<UnrealPackage> unrealPackageSerializerFor)
    {
        UnrealPackageSerializerFor = unrealPackageSerializerFor;
    }

    /// <summary>
    ///     The paths to search for related packages. Will default to cwd if no path is specified
    /// </summary>
    public List<string> SearchPaths { get; init; } = new();

    /// <summary>
    ///     The file extensions to search for. Do not include the dot, just the extension
    /// </summary>
    public List<string> Extensions { get; init; } = new() { "u", "upk" };

    /// <summary>
    ///     If the loader should auto link all packages or not
    /// </summary>
    public bool GraphLinkPackages { get; set; } = true;

    /// <summary>
    ///     The serializer to use when loading packages
    /// </summary>
    public IStreamSerializerFor<UnrealPackage> UnrealPackageSerializerFor { get; init; }

    /// <summary>
    ///     Optional. Use a package unpacker to enable auto-loading of packed\compressed packages
    /// </summary>
    public IPackageUnpacker PackageUnpacker { get; set; } = new NeverUnpackUnpacker();


    /// <summary>
    ///     Factory used to create serializers for all UObjects in the package
    /// </summary>
    public IObjectSerializerFactory? ObjectSerializerFactory { get; set; }
}

/// <summary>
///     The PackageCache can find and cache packages for reuse. Especially useful when resolving resolving import object
///     (hence the name)
/// </summary>
public class PackageCache : IPackageCache
{
    private readonly Dictionary<string, UnrealPackage> _cachedPackages = new();
    private readonly PackageCacheOptions _options;

    /// <summary>
    ///     Constructs a configured PackageCache
    /// </summary>
    /// <param name="options"></param>
    public PackageCache(PackageCacheOptions options)
    {
        _options = options;
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


        Matcher matcher = new();
        matcher.AddIncludePatterns(_options.Extensions.Select(ext => $"{packageName}.{ext}"));
        var matchedFiles = new List<string>();
        foreach (var searchPath in _options.SearchPaths)
        {
            //var directoryPath = "D:\\Projects\\RL UPKSuite\\Core.Test\\TestData\\UDK";
            matchedFiles.AddRange(matcher.GetResultsInFullPath(searchPath));
        }

        switch (matchedFiles.Count)
        {
            case 0:
                return null;
            case > 1:
                throw new InvalidDataException($"Too many matches packages found: {string.Join(',', matchedFiles)}");
        }

        var packageStream = File.OpenRead(matchedFiles[0]);

        UnrealPackage unrealPackage;
        var loadOptions = new UnrealPackageOptions(_options.UnrealPackageSerializerFor, packageName, this, _options.ObjectSerializerFactory);
        if (_options.PackageUnpacker.IsPackagePacked(packageStream))
        {
            var unpackedStream = new MemoryStream();
            _options.PackageUnpacker.Unpack(packageStream, unpackedStream);
            unpackedStream.Position = 0;
            unrealPackage = UnrealPackage.DeserializeAndInitialize(unpackedStream, loadOptions);
        }
        else
        {
            unrealPackage = UnrealPackage.DeserializeAndInitialize(packageStream, loadOptions);
        }


        // TODO: Reconsider if the import resolver should link the objects.
        if (_options.GraphLinkPackages)
        {
            unrealPackage.GraphLink();
        }

        _cachedPackages.Add(packageName, unrealPackage);

        return unrealPackage;
    }

    /// <inheritdoc />
    public UnrealPackage GetCachedPackage(string packageName)
    {
        return _cachedPackages[packageName];
    }

    /// <inheritdoc />
    public void AddPackage(UnrealPackage package)
    {
        _cachedPackages.Add(package.PackageName, package);
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
}