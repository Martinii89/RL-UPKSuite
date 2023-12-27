using System.IO;

using Core;
using Core.Classes.Compression;
using Core.Types;
using Core.Utility.Export;

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
        try
        {
            if (ProcessFileImpl(fileReference.FilePath))
            {
                fileReference.ProcessSuccess = true;
            }

            return true;
        }
        catch (Exception e)
        {
            fileReference.ProcessSuccess = false;
            _logger?.LogError("Failed to convert package {PackageName}: Exception:{ExceptionMessage}",
                fileReference.FilePath,
                e.Message);
            return false;
        }
    }

    private bool ProcessFileImpl(string file)
    {
        string inputFileName = Path.GetFileNameWithoutExtension(file);
        using MemoryStream convertedStream = new();
        UnrealPackage? package = _packageLoader.LoadPackage(file, inputFileName);
        if (package is null)
        {
            _logger?.LogError("Failed to load package {InputFileName}", inputFileName);
            return false;
        }

        PackageExporter exporter = _packageExporterFactory.Create(package, convertedStream);
        exporter.ExportPackage();
        convertedStream.Position = 0;
        string outputFilePath = GetOutputFilePath(file);
        FileInfo fileInfo = new(outputFilePath);
        if (!fileInfo.Exists)
        {
            fileInfo.Directory?.Create();
        }

        using FileStream outputFile = File.Create(outputFilePath);
        if (_options.Compress && _compressor is not null)
        {
            MemoryStream compressedStream = new();
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
        string fileName = Path.GetFileNameWithoutExtension(file) + _options.Suffix + Path.GetExtension(file);
        return Path.Combine(_options.OutputDirectory, fileName);
    }
}