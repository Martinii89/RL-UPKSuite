﻿// See https://aka.ms/new-console-template for more information

using System.Reflection;
using Core;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Serialization.RocketLeague;
using Core.Types;
using Core.Types.PackageTables;
using Core.Utility;
using Core.Utility.Export;
using Microsoft.Extensions.DependencyInjection;

//var parseResult = Parser.Default.ParseArguments<BatchProcessOptions>(args);
//parseResult.WithParsed(BatchProcess);
var assemblyLocation = Assembly.GetExecutingAssembly().Location;
var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
Environment.CurrentDirectory = currentDirectory;

var inputFile = args[0];
var inputFileName = Path.GetFileNameWithoutExtension(inputFile);
var inputDirectory = Path.GetDirectoryName(inputFile);
ArgumentNullException.ThrowIfNull(inputDirectory);
var outputFileName = $"{Path.GetFileNameWithoutExtension(inputFile)}_converted{Path.GetExtension(inputFile)}";
//var outputFile = Path.Combine(Path.GetDirectoryName(inputFile) ?? throw new InvalidOperationException(), outputFileName);
var outputFile = Path.Combine("Converted packages", outputFileName);
var directoryInfo = new FileInfo(outputFile).Directory;
if (directoryInfo == null)
{
    throw new FileNotFoundException();
}

directoryInfo.Create();


//Boilerplate service setup

Console.WriteLine("Service setup..");
using var convertedStream = new MemoryStream();
using var outputStream = File.OpenWrite(outputFile);

var rlServices = GetRLSerializerCollection();
var udkServices = GetUdkSerializerCollection();

var rlFileSummarySerializer = rlServices.GetRequiredService<IStreamSerializer<FileSummary>>();
var rlPackageSerializer = rlServices.GetRequiredService<IStreamSerializer<UnrealPackage>>();
var rLobjectSerializerFactory = rlServices.GetRequiredService<IObjectSerializerFactory>();

var unpacker = new PackageUnpacker(rlFileSummarySerializer, new DecryptionProvider("keys.txt"));
var nativeFactory = new NativeClassFactory();
var options = new PackageCacheOptions(rlPackageSerializer, nativeFactory)
{
    SearchPaths = { inputDirectory },
    GraphLinkPackages = true,
    PackageUnpacker = unpacker,
    NativeClassFactory = nativeFactory,
    ObjectSerializerFactory = rLobjectSerializerFactory,
    PackageBlacklist = { "EngineMaterials", "EngineResources" }
};
var packageCache = new PackageCache(options);


// Parse the package
var loader = new PackageLoader(rlPackageSerializer, packageCache, unpacker, nativeFactory, rLobjectSerializerFactory);
Console.WriteLine("Parsing.. (this will take a while)");
var package = loader.LoadPackage(inputFile, inputFileName);

// Create the exporter
var exporter = new PackageExporter(package, convertedStream,
    udkServices.GetRequiredService<IStreamSerializer<FileSummary>>(),
    udkServices.GetRequiredService<IStreamSerializer<NameTableItem>>(),
    udkServices.GetRequiredService<IStreamSerializer<ImportTableItem>>(),
    udkServices.GetRequiredService<IStreamSerializer<ExportTableItem>>(),
    udkServices.GetRequiredService<IStreamSerializer<ObjectIndex>>(),
    udkServices.GetRequiredService<IStreamSerializer<FName>>());

var UDKobjectSerializerFactory = udkServices.GetRequiredService<IObjectSerializerFactory>();

// Export and save the package
Console.WriteLine("Converting..");
exporter.ExportPackage(UDKobjectSerializerFactory);
convertedStream.Position = 0;
Console.WriteLine($"Writing to {outputFile}..");
convertedStream.CopyTo(outputStream);


IServiceProvider GetUdkSerializerCollection()
{
    var serviceCollection = new ServiceCollection();
    serviceCollection.UseSerializers(typeof(UnrealPackage), new SerializerOptions());
    serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
    var services = serviceCollection.BuildServiceProvider();
    return services;
}

IServiceProvider GetRLSerializerCollection()
{
    var serviceCollection = new ServiceCollection();
    var rlFileVersionTag = RocketLeagueBase.FileVersion;
    serviceCollection.UseSerializers(typeof(UnrealPackage), new SerializerOptions(rlFileVersionTag));
    serviceCollection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
    var services = serviceCollection.BuildServiceProvider();
    return services;
}