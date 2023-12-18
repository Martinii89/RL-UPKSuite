using System.IO;

using Core;
using Core.Classes.Compression;

using RLUpkSuite.Config;
using RLUpkSuite.ViewModels;

namespace RLUpkSuite.PackageConversion;

internal class PackageConverter
{
    private readonly PackageCompressor? _compressor;
    private readonly ConversionConfig _options;
    private readonly PackageExporterFactory _packageExporterFactory;
    private readonly PackageLoader _packageLoader;

    private IEnumerable<FileReference> _files;

    public PackageConverter(ConversionConfig options, PackageExporterFactory packageExporterFactory, PackageLoader packageLoader,
        PackageCompressor? compressor)
    {
        _options = options;
        _compressor = compressor;
        _packageExporterFactory = packageExporterFactory;
        _packageLoader = packageLoader;
    }

    public void Start()
    {
        foreach (var file in _files)
        {
            try
            {
                if (ProcessFile(file.FilePath))
                {
                    file.ProcessSuccess = true;
                }
            }
            catch (Exception e)
            {
                //TODO: log and show error to user
                Console.WriteLine(e);
                throw;
            }
        }
    }

    private bool ProcessFile(string file)
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

    public void SetFiles(IEnumerable<FileReference> fileReferences)
    {
        this._files = fileReferences;
    }
}