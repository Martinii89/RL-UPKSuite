using Core;
using Core.Classes.Compression;

using Microsoft.Extensions.FileSystemGlobbing;

namespace RLToUdkConverter;

internal class PackageConverter(
    PackageConversionOptions options,
    PackageExporterFactory packageExporterFactory,
    PackageLoader packageLoader,
    PackageCompressor? compressor)
{
    public void ProcessFile(string file)
    {
        var inputFileName = Path.GetFileNameWithoutExtension(file);
        using var convertedStream = new MemoryStream();
        var package = packageLoader.LoadPackage(file, inputFileName, false);
        if (package is null)
        {
            Console.WriteLine($"Failed to load package {inputFileName}");
            return;
        }

        var exporter = packageExporterFactory.Create(package, convertedStream);
        exporter.ExportPackage();
        convertedStream.Position = 0;
        var outputFilePath = GetOutputFilePath(file);
        var fileInfo = new FileInfo(outputFilePath);
        if (!fileInfo.Exists)
        {
            fileInfo.Directory?.Create();
        }

        using var outputFile = File.Create(outputFilePath);
        if (options.Compress && compressor is not null)
        {
            var compressedStream = new MemoryStream();
            compressor.CompressFile(convertedStream, compressedStream);
            compressedStream.Position = 0;
            compressedStream.CopyTo(outputFile);
        }
        else
        {
            convertedStream.CopyTo(outputFile);
        }
    }


    private string GetOutputFilePath(string file)
    {
        var fileName = Path.GetFileNameWithoutExtension(file) + options.Suffix + Path.GetExtension(file);
        return Path.Combine(options.OutputDirectory, fileName);
    }
}