using System.Diagnostics;

using CommandLine;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;

using RlUpk.Core;
using RlUpk.Core.Classes;
using RlUpk.Core.RocketLeague;
using RlUpk.Core.RocketLeague.Decryption;
using RlUpk.Core.Serialization;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;
using RlUpk.Core.Serialization.RocketLeague;
using RlUpk.Core.Types;
using RlUpk.Core.Utility;
using RlUpk.MapBuilder.Cli;


var parseResult = Parser.Default.ParseArguments<MapBuilderOptions>(args);
await parseResult.WithParsedAsync(BatchProcess);
return;

IEnumerable<string> FindGlobFiles(string folder, IEnumerable<string> optionsGlobPatterns)
{
    var matcher = new Matcher();
    matcher.AddIncludePatterns(optionsGlobPatterns);
    var fileMatches = matcher.GetResultsInFullPath(folder);
    return fileMatches;
}

string FindDependantPackageDirectory(MapBuilderOptions options)
{
    if (!string.IsNullOrEmpty(options.ImportPackagesDirectory))
    {
        return options.ImportPackagesDirectory;
    }

    var firstFile = options.Files?.FirstOrDefault();
    if (firstFile is null)
    {
        throw new InvalidDataException("No files to process");
    }

    return Path.GetDirectoryName(firstFile) ?? throw new InvalidDataException("Unable to resolve the folder for the packages");
}

async Task BatchProcess(MapBuilderOptions options)
{
    var files = new List<string>();
    if (string.IsNullOrWhiteSpace(options.ImportPackagesDirectory))
    {
        options.ImportPackagesDirectory = FindDependantPackageDirectory(options);
    }

    ArgumentException.ThrowIfNullOrWhiteSpace(options.ImportPackagesDirectory);

    if (options.GlobPatterns is not null && !string.IsNullOrWhiteSpace(options.ImportPackagesDirectory))
    {
        files.AddRange(FindGlobFiles(options.ImportPackagesDirectory, options.GlobPatterns));
    }

    if (options.Files != null)
    {
        files.AddRange(options.Files);
    }

    MapBuilderProcessor mapBuilderProcessor = CreateMapBuilderProcessor(options);
    IEnumerable<string> distinctFiles = files.Distinct().ToList();
    await Parallel.ForEachAsync(distinctFiles, async (file, _) => await mapBuilderProcessor.ExportAssets(file, options.DecryptedFolder, options.AssetsFolder, options.OutputFolder));
    foreach (var file in distinctFiles)
    {
        try
        {
            var sw = Stopwatch.StartNew();
            mapBuilderProcessor.ProcessFile(file, options.AssetsFolder, options.OutputFolder);
            sw.Stop();
            Console.WriteLine($"Processed: {file} in {sw.Elapsed.TotalMilliseconds} ms");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception while processing {file}: {e.Message}");
        }
    }
}

MapBuilderProcessor CreateMapBuilderProcessor(MapBuilderOptions mapBuilderOptions)
{
    {
        var rlServices = GetRLSerializerCollection();

        var rlFileSummarySerializer = rlServices.GetRequiredService<IStreamSerializer<FileSummary>>();
        var rlPackageSerializer = rlServices.GetRequiredService<IStreamSerializer<UnrealPackage>>();
        var rLobjectSerializerFactory = rlServices.GetRequiredService<IObjectSerializerFactory>();

        var unpacker = new PackageUnpacker(rlFileSummarySerializer, new DecryptionProvider(mapBuilderOptions.KeysPath));
        var nativeFactory = new NativeClassFactory();

        var cacheOptions = new PackageCacheOptions(rlPackageSerializer, nativeFactory)
        {
            SearchPaths = { mapBuilderOptions.ImportPackagesDirectory },
            GraphLinkPackages = true,
            PackageUnpacker = unpacker,
            NativeClassFactory = nativeFactory,
            ObjectSerializerFactory = rLobjectSerializerFactory,
            PackageBlacklist = { "EngineMaterials", "EngineResources" }
        };
        var packageCache = new PackageCache(cacheOptions);

        var loader = new PackageLoader(rlPackageSerializer, packageCache, unpacker, nativeFactory, rLobjectSerializerFactory);
        return new MapBuilderProcessor(loader, unpacker);
    }

    IServiceProvider GetRLSerializerCollection()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSerializers(typeof(UnrealPackage), new SerializerOptions(RocketLeagueBase.FileVersion));
        serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
        var services = serviceCollection.BuildServiceProvider();
        return services;
    }




}