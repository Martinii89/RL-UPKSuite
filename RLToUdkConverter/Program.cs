using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;

using CommandLine;
using Core;
using Core.Classes.Compression;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Serialization.Default;
using Core.Serialization.RocketLeague;
using Core.Types;
using Core.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;

using RLToUdkConverter;
using ExportTableItemSerializer = Core.Serialization.Default.ExportTableItemSerializer;

var parseResult = Parser.Default.ParseArguments<PackageConversionOptions>(args);
await parseResult.WithParsedAsync(BatchProcess);
return;

IEnumerable<string> FindGlobFiles(string folder, IEnumerable<string> optionsGlobPatterns)
{
    var matcher = new Matcher();
    matcher.AddIncludePatterns(optionsGlobPatterns);
    var fileMatches = matcher.GetResultsInFullPath(folder);
    return fileMatches;
}

string FindDependantPackageDirectory(PackageConversionOptions options)
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

 async Task BatchProcess(PackageConversionOptions options)
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

    var processQueue = new ConcurrentQueue<string>(files.Distinct());
    var sw = Stopwatch.StartNew();
    IEnumerable<Task> processTasks = Enumerable.Range(0, options.Threads).Select(i =>
    {
        return Task.Run(() =>
        {
            PackageConverter packageConverter = CreatePackageConverter(options);
            while (processQueue.TryDequeue(out string? file))
            {
                try
                {
                    var sw = Stopwatch.StartNew();
                    packageConverter!.ProcessFile(file);
                    sw.Stop();
                    if (sw.Elapsed.TotalSeconds > 1)
                        Console.WriteLine($"Processed: {file} in {sw.Elapsed.TotalMilliseconds} ms");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception while processing {file}: {e.Message}");
                }
            }
        });
    });
    await Task.WhenAll(processTasks);
    Console.WriteLine($"Done in {sw.Elapsed.TotalSeconds}");
}

 PackageConverter CreatePackageConverter(PackageConversionOptions packageConversionOptions)
 {
     {
         var rlServices = GetRLSerializerCollection();
         var udkServices = GetUdkSerializerCollection();

         var rlFileSummarySerializer = rlServices.GetRequiredService<IStreamSerializer<FileSummary>>();
         var rlPackageSerializer = rlServices.GetRequiredService<IStreamSerializer<UnrealPackage>>();
         var rLobjectSerializerFactory = rlServices.GetRequiredService<IObjectSerializerFactory>();

         var unpacker = new PackageUnpacker(rlFileSummarySerializer, new DecryptionProvider(packageConversionOptions.KeysPath));
         var nativeFactory = new NativeClassFactory();

         var cacheOptions = new PackageCacheOptions(rlPackageSerializer, nativeFactory)
         {
             SearchPaths = { packageConversionOptions.ImportPackagesDirectory },
             GraphLinkPackages = true,
             PackageUnpacker = unpacker,
             NativeClassFactory = nativeFactory,
             ObjectSerializerFactory = rLobjectSerializerFactory,
             PackageBlacklist = { "EngineMaterials", "EngineResources" }
         };
         var packageCache = new PackageCache(cacheOptions);

         var loader = new PackageLoader(rlPackageSerializer, packageCache, unpacker, nativeFactory, rLobjectSerializerFactory);
         var exporterFactory = udkServices.GetRequiredService<PackageExporterFactory>();

         var packageConverter1 = new PackageConverter(packageConversionOptions, exporterFactory, loader, packageConversionOptions.Compress ? GetDefaultPackageCompressor() : null);
         return packageConverter1;
     }

     IServiceProvider GetRLSerializerCollection()
     {
         var serviceCollection = new ServiceCollection();
         serviceCollection.AddSerializers(typeof(UnrealPackage), new SerializerOptions(RocketLeagueBase.FileVersion));
         serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
         var services = serviceCollection.BuildServiceProvider();
         return services;
     }

     IServiceProvider GetUdkSerializerCollection()
     {
         var serviceCollection = new ServiceCollection();
         serviceCollection.AddSerializers(typeof(UnrealPackage), new SerializerOptions());
         serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
         serviceCollection.AddSingleton<PackageExporterFactory>();
         var services = serviceCollection.BuildServiceProvider();
         return services;
     }

     PackageCompressor GetDefaultPackageCompressor()
     {
         var headerSerializer = FileSummarySerializer.GetDefaultSerializer();
         var exportTableIteSerializer =
             new ExportTableItemSerializer(new FNameSerializer(), new ObjectIndexSerializer(), new FGuidSerializer());
         return new PackageCompressor(headerSerializer, exportTableIteSerializer, new FCompressedChunkinfoSerializer());
     }
 }