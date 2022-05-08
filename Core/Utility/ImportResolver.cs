using Core.Serialization;
using Core.Types;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Core.Utility;

/// <summary>
///     Configuration object for the <see cref="ImportResolver" />
/// </summary>
public class ImportResolverOptions
{
    /// <summary>
    ///     The serializer is required. The other members are optional
    /// </summary>
    /// <param name="unrealPackageSerializerFor"></param>
    public ImportResolverOptions(IStreamSerializerFor<UnrealPackage> unrealPackageSerializerFor)
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
    ///     The serializer to use when loading packages
    /// </summary>
    public IStreamSerializerFor<UnrealPackage> UnrealPackageSerializerFor { get; init; }
}

/// <summary>
///     The ImportResolver can find and cache packages for reuse. Especially useful when resolving resolving import object
///     (hence the name)
/// </summary>
public class ImportResolver : IImportResolver
{
    private readonly Dictionary<string, UnrealPackage> _cachedPackages = new();
    private readonly ImportResolverOptions _options;

    /// <summary>
    ///     Constructs a configured ImportResolver
    /// </summary>
    /// <param name="options"></param>
    public ImportResolver(ImportResolverOptions options)
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
        // TODO detect package type (UDK, Psyonix cooked)

        var package = _options.UnrealPackageSerializerFor.Deserialize(packageStream);
        package.ImportResolver = this;
        package.PostDeserializeInitialize(packageName);

        // TODO: Reconsider if the import resolver should link the objects.
        package.GraphLink();

        _cachedPackages.Add(packageName, package);

        return package;
    }
}