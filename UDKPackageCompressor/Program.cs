// See https://aka.ms/new-console-template for more information

using Core.Classes.Compression;
using Core.Serialization.Default;

if (args.Length < 1)
{
    Console.WriteLine("Usage: Provide the path to a upk file as the one and only argument");
    return;
}

var inputFile = args[0];
var outputFileName = $"{Path.GetFileNameWithoutExtension(inputFile)}_compressed{Path.GetExtension(inputFile)}";
var outputFile = Path.Combine(Path.GetDirectoryName(inputFile) ?? throw new InvalidOperationException(), outputFileName);

try
{
    var headerSerializer = FileSummarySerializer.GetDefaultSerializer();
    var exportTableIteSerializer =
        new ExportTableItemSerializer(new FNameSerializer(), new ObjectIndexSerializer(), new Int32Serializer(), new FGuidSerializer());
    using var inputPackageStream = File.OpenRead(inputFile);
    using var outputStream = File.OpenWrite(outputFile);
    var compressor = new PackageCompressor(headerSerializer, exportTableIteSerializer, new FCompressedChunkinfoSerializer());
    compressor.CompressFile(inputPackageStream, outputStream);
    Console.WriteLine($"Package compressed and written to {outputFile}");
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}