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
using RLToUdkConverter;
using ExportTableItemSerializer = Core.Serialization.Default.ExportTableItemSerializer;

var parseResult = Parser.Default.ParseArguments<PackageConversionOptions>(args);
parseResult.WithParsed(BatchProcess);


IServiceProvider GetUdkSerializerCollection()
{
    var serviceCollection = new ServiceCollection();
    serviceCollection.UseSerializers(typeof(UnrealPackage), new SerializerOptions());
    serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
    serviceCollection.AddSingleton<PackageExporterFactory>();
    var services = serviceCollection.BuildServiceProvider();
    return services;
}

IServiceProvider GetRLSerializerCollection()
{
    var serviceCollection = new ServiceCollection();
    serviceCollection.UseSerializers(typeof(UnrealPackage), new SerializerOptions(RocketLeagueBase.FileVersion));
    serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
    var services = serviceCollection.BuildServiceProvider();
    return services;
}

string FindDependantPackageDirectory(PackageConversionOptions options)
{
    if (!string.IsNullOrEmpty(options.ImportPackagesDirectory))
    {
        return options.ImportPackagesDirectory;
    }

    var firstFile = options.Files.FirstOrDefault();
    if (firstFile is null)
    {
        throw new InvalidDataException("No files to process");
    }

    return Path.GetDirectoryName(firstFile) ?? throw new InvalidDataException("Unable to resolve the folder for the packages");
}

PackageCompressor GetDefaultPackageCompressor()
{
    var headerSerializer = FileSummarySerializer.GetDefaultSerializer();
    var exportTableIteSerializer =
        new ExportTableItemSerializer(new FNameSerializer(), new ObjectIndexSerializer(), new FGuidSerializer());
    return new PackageCompressor(headerSerializer, exportTableIteSerializer, new FCompressedChunkinfoSerializer());
}

void BatchProcess(PackageConversionOptions options)
{
    var rlServices = GetRLSerializerCollection();
    var udkServices = GetUdkSerializerCollection();

    var rlFileSummarySerializer = rlServices.GetRequiredService<IStreamSerializer<FileSummary>>();
    var rlPackageSerializer = rlServices.GetRequiredService<IStreamSerializer<UnrealPackage>>();
    var rLobjectSerializerFactory = rlServices.GetRequiredService<IObjectSerializerFactory>();

    var unpacker = new PackageUnpacker(rlFileSummarySerializer, new DecryptionProvider(options.KeysPath));
    var nativeFactory = new NativeClassFactory();

    options.ImportPackagesDirectory = FindDependantPackageDirectory(options);

    var cacheOptions = new PackageCacheOptions(rlPackageSerializer, nativeFactory)
    {
        SearchPaths = { options.ImportPackagesDirectory },
        GraphLinkPackages = true,
        PackageUnpacker = unpacker,
        NativeClassFactory = nativeFactory,
        ObjectSerializerFactory = rLobjectSerializerFactory,
        PackageBlacklist = { "EngineMaterials", "EngineResources" }
    };
    var packageCache = new PackageCache(cacheOptions);

    var loader = new PackageLoader(rlPackageSerializer, packageCache, unpacker, nativeFactory, rLobjectSerializerFactory);
    var exporterFactory = udkServices.GetRequiredService<PackageExporterFactory>();

    var packageConverter = new PackageConverter(options, exporterFactory, loader, options.Compress ? GetDefaultPackageCompressor() : null);
    packageConverter.Start();
}