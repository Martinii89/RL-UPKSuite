using Core.Serialization;
using Core.Types;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Core.Utility;

public class ImportResolverOptions
{
    public ImportResolverOptions(IStreamSerializerFor<UnrealPackage> unrealPackageSerializerFor)
    {
        UnrealPackageSerializerFor = unrealPackageSerializerFor;
    }

    public List<string> SearchPaths { get; init; } = new();
    public List<string> Extensions { get; init; } = new() { "u", "upk" };

    public IStreamSerializerFor<UnrealPackage> UnrealPackageSerializerFor { get; init; }
}

public class ImportResolver : IImportResolver
{
    private readonly Dictionary<string, UnrealPackage> _cachedPackages = new();
    private readonly ImportResolverOptions _options;

    public ImportResolver(ImportResolverOptions options)
    {
        _options = options;
    }

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
        package.PostDeserializeInitialize(packageName);

        _cachedPackages.Add(packageName, package);

        return package;
    }
}