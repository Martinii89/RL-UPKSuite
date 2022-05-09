// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using CommandLine;
using Core;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization.Default;
using Core.Utility;
using Decryptor;

// Arrange
var fileSummarySerializerFor = FileSummarySerializer.GetDefaultSerializer();
var nameTableItemSerializer = new NameTableItemSerializer();
var importTableItemSerializer = new ImportTableItemSerializer(new FNameSerializer(), new ObjectIndexSerializer());
var exportTablItemeSerializer = new ExportTableItemSerializer(new FNameSerializer(), new ObjectIndexSerializer(), new Int32Serializer(), new FGuidSerializer());
var serializer = new UnrealPackageSerializer(fileSummarySerializerFor, nameTableItemSerializer, importTableItemSerializer, exportTablItemeSerializer);
var options = new ImportResolverOptions(serializer) { SearchPaths = { @"D:\Projects\RL UPKSuite\Core.Test\TestData\UDK\" }, GraphLinkPackages = false };

var stopwatch = new Stopwatch();
stopwatch.Start();
for (var i = 0; i < 100; i++)
{
    var packageCache = new PackageCache(options);
    var loader = new PackageLoader(serializer, packageCache);
    loader.LoadPackage("D:\\Projects\\RL UPKSuite\\Core.Test\\TestData\\UDK\\TAGame.u", "TAGame");
    var tagame = loader.GetPackage("TAGame");
}

Console.WriteLine($"CrossPackage Load took {stopwatch.Elapsed.TotalMilliseconds / 100} ms");
return;
// Act

// Assert 


return;
var parseResult = Parser.Default.ParseArguments<BatchProcessOptions>(args);

parseResult.WithParsed(BatchProcess);


void BatchProcess(BatchProcessOptions options)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var inputFolder = options.InputDirectory;
    var outputFolder = options.OutputDirectory;
    var files = Directory.EnumerateFiles(inputFolder, options.GlobPattern);
    var decryptionProvider = new DecryptionProvider("keys.txt");


    Parallel.ForEach(files, file =>
    {
        if (file.EndsWith("_decrypted.upk"))
        {
            Console.Error.WriteLine("File is already decrypted.");
            return;
        }

        if (file.Contains("RefShaderCache"))
        {
            Console.WriteLine("Skipping shadercache");
            return;
        }

        var inputFileName = Path.GetFileNameWithoutExtension(file);
        var outputFilePath = Path.Combine(outputFolder, inputFileName + "_decrypted.upk");
        new FileInfo(outputFilePath).Directory.Create();


        using var fileStream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var decryptedStream = File.OpenWrite(outputFilePath);
        var upkFile = new PackageUnpacker(fileStream, decryptedStream, decryptionProvider, fileSummarySerializerFor);
        if (!upkFile.Valid)
        {
            Console.WriteLine($"Failed decrypting {inputFileName} probably unknown decryption key");
        }
    });
    Console.WriteLine($"Decryption took {stopwatch.Elapsed.TotalMilliseconds} ms");
}