using Core.Serialization;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Utility;

public class PackageExporter
{
    private readonly Stream _exportStream;
    private readonly IStreamSerializer<FileSummary> _fileSummarySerializer;
    private readonly IStreamSerializer<NameTableItem> _nameTableItemSerializer;

    private readonly UnrealPackage _package;

    public PackageExporter(UnrealPackage package, Stream exportStream, IStreamSerializer<FileSummary> fileSummarySerializer,
        IStreamSerializer<NameTableItem> nameTableItemSerializer)
    {
        _fileSummarySerializer = fileSummarySerializer;
        _nameTableItemSerializer = nameTableItemSerializer;
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

    /// <summary>
    ///     Writes the name table to the current position of the stream.
    /// </summary>
    public void ExportNameTable()
    {
        _nameTableItemSerializer.WriteTArray(_exportStream, _package.NameTable.ToArray(), StreamSerializerForExtension.ArraySizeSerialization.NoSize);
    }
}