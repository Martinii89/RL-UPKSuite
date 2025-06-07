// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using CommandLine;

using RlUpk.Core.RocketLeague;
using RlUpk.Core.RocketLeague.Decryption;
using RlUpk.Core.Serialization.Default;
using RlUpk.Decryptor;

var parseResult = Parser.Default.ParseArguments<BatchProcessOptions>(args);
parseResult.WithParsed(BatchProcess);


void BatchProcess(BatchProcessOptions options)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var inputFolder = options.InputDirectory;
    var outputFolder = options.OutputDirectory;
    var files = Directory.EnumerateFiles(inputFolder, options.GlobPattern);
    var decryptionProvider = new DecryptionProvider(options.KeysPath);


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
        var upkFile = new RLPackageUnpacker(fileStream, decryptionProvider, FileSummarySerializer.GetDefaultSerializer());
        upkFile.Unpack(decryptedStream);
        if (!upkFile.Valid)
        {
            Console.WriteLine($"Failed decrypting {inputFileName} probably unknown decryption key");
        }
    });
    Console.WriteLine($"Decryption took {stopwatch.Elapsed.TotalMilliseconds} ms");
}
