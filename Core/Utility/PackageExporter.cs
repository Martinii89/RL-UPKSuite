using Core.Serialization;
using Core.Types;

namespace Core.Utility;

public class PackageExporter
{
    private readonly Stream _exportStream;
    private readonly IStreamSerializer<FileSummary> _fileSummarySerializer;

    private readonly UnrealPackage _package;

    public PackageExporter(UnrealPackage package, Stream exportStream, IStreamSerializer<FileSummary> fileSummarySerializer)
    {
        _fileSummarySerializer = fileSummarySerializer;
        _exportStream = exportStream;
        _package = package;
    }

    /// <summary>
    ///     Writes the package header information to the start of the export stream
    /// </summary>
    public void ExportHeader()
    {
        _exportStream.Position = 0;
        _fileSummarySerializer.Serialize(_exportStream, _package.Header);
    }
}