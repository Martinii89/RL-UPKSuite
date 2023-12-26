using System.IO;

using Core;
using Core.Classes.Compression;

using Microsoft.Extensions.Logging;

using RLUpkSuite.Config;
using RLUpkSuite.ViewModels;

namespace RLUpkSuite.PackageConversion;

public class PackageConverter
{
    private readonly PackageCompressor? _compressor;

    private readonly ILogger<PackageConverter>? _logger;

    private readonly ConversionConfig _options;

    private readonly PackageExporterFactory _packageExporterFactory;

    private readonly PackageLoader _packageLoader;

    public PackageConverter(ConversionConfig options, PackageExporterFactory packageExporterFactory,
        PackageLoader packageLoader,
        PackageCompressor? compressor, ILogger<PackageConverter>? logger)
    {
        _options = options;
        _compressor = compressor;
        _logger = logger;
        _packageExporterFactory = packageExporterFactory;
        _packageLoader = packageLoader;
    }

    public bool ProcessFile(FileReference fileReference)
    {
        // try
        {
            if (ProcessFileImpl(fileReference.FilePath))
            {
                fileReference.ProcessSuccess = true;
            }

            return true;
        }
        // catch (Exception e)
        // {
        //     fileReference.ProcessSuccess = false;
        //     _logger?.LogError("Failed to convert package {PackageName}: Exception:{ExceptionMessage}",
        //         fileReference.FilePath,
        //         e.Message);
        //     return false;
        // }
    }

    private bool ProcessFileImpl(string file)
    {
        var inputFileName = Path.GetFileNameWithoutExtension(file);
        using var convertedStream = new MemoryStream();
        var package = _packageLoader.LoadPackage(file, inputFileName);
        if (package is null)
        {
            Console.WriteLine($"Failed to load package {inputFileName}");
            return false;
        }

        var exporter = _packageExporterFactory.Create(package, convertedStream);
        exporter.ExportPackage();
        convertedStream.Position = 0;
        var outputFilePath = GetOutputFilePath(file);
        var fileInfo = new FileInfo(outputFilePath);
        if (!fileInfo.Exists)
        {
            fileInfo.Directory?.Create();
        }

        using var outputFile = File.Create(outputFilePath);
        if (_options.Compress && _compressor is not null)
        {
            var compressedStream = new MemoryStream();
            _compressor.CompressFile(convertedStream, compressedStream);
            compressedStream.Position = 0;
            compressedStream.CopyTo(outputFile);
        }
        else
        {
            convertedStream.CopyTo(outputFile);
        }

        return true;
    }


    private string GetOutputFilePath(string file)
    {
        var fileName = Path.GetFileNameWithoutExtension(file) + _options.Suffix + Path.GetExtension(file);
        return Path.Combine(_options.OutputDirectory, fileName);
    }
}