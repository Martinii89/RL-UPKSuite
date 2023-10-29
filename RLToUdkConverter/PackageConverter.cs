using Core;
using Core.Classes.Compression;

namespace RLToUdkConverter;

internal class PackageConverter
{
    private readonly PackageCompressor? _compressor;
    private readonly PackageConversionOptions _options;
    private readonly PackageExporterFactory _packageExporterFactory;
    private readonly PackageLoader _packageLoader;

    public PackageConverter(PackageConversionOptions options, PackageExporterFactory packageExporterFactory, PackageLoader packageLoader,
        PackageCompressor? compressor)
    {
        _options = options;
        _compressor = compressor;
        _packageExporterFactory = packageExporterFactory;
        _packageLoader = packageLoader;
    }

    public void Start()
    {
        foreach (var file in _options.Files)
        {
            ProcessFile(file);
        }
    }

    private void ProcessFile(string file)
    {
        var inputFileName = Path.GetFileNameWithoutExtension(file);
        using var convertedStream = new MemoryStream();
        var package = _packageLoader.LoadPackage(file, inputFileName);
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
    }


    private string GetOutputFilePath(string file)
    {
        var fileName = Path.GetFileNameWithoutExtension(file) + _options.Suffix + Path.GetExtension(file);
        return Path.Combine(_options.OutputDirectory, fileName);
    }
}