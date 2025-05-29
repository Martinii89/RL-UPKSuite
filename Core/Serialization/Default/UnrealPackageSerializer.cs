using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;
using RlUpk.Core.Types;
using RlUpk.Core.Types.PackageTables;

namespace RlUpk.Core.Serialization.Default;

/// <summary>
///     Serializes the UnrealPackage data. It does not link up or create any objects, just the header and table data is
///     serialized.
/// </summary>
public class UnrealPackageSerializer : IStreamSerializer<UnrealPackage>
{
    private readonly IStreamSerializer<ExportTableItem> _exportTablItemeSerializer;
    private readonly IStreamSerializer<FileSummary> _fileSummarySerializer;
    private readonly IStreamSerializer<ImportTableItem> _importTableItemSerializer;
    private readonly IStreamSerializer<NameTableItem> _nameTableItemSerializer;

    /// <summary>
    ///     Constructs a serializer capable of serializing a UnrealPackage by giving it all the required sub serializers
    /// </summary>
    /// <param name="fileSummarySerializer"></param>
    /// <param name="nameTableItemSerializer"></param>
    /// <param name="importTableItemSerializer"></param>
    /// <param name="exportTablItemeSerializer"></param>
    public UnrealPackageSerializer(IStreamSerializer<FileSummary> fileSummarySerializer, IStreamSerializer<NameTableItem> nameTableItemSerializer,
        IStreamSerializer<ImportTableItem> importTableItemSerializer, IStreamSerializer<ExportTableItem> exportTablItemeSerializer)
    {
        _fileSummarySerializer = fileSummarySerializer;
        _nameTableItemSerializer = nameTableItemSerializer;
        _importTableItemSerializer = importTableItemSerializer;
        _exportTablItemeSerializer = exportTablItemeSerializer;
    }

    /// <inheritdoc />
    public UnrealPackage Deserialize(Stream stream)
    {
        var package = new UnrealPackage();
        package.Header = _fileSummarySerializer.Deserialize(stream);

        stream.Seek(package.Header.NameOffset, SeekOrigin.Begin);
        _nameTableItemSerializer.ReadTArrayToList(stream, package.NameTable, package.Header.NameCount);

        stream.Seek(package.Header.ImportOffset, SeekOrigin.Begin);
        _importTableItemSerializer.ReadTArrayToList(stream, package.ImportTable, package.Header.ImportCount);

        stream.Seek(package.Header.ExportOffset, SeekOrigin.Begin);
        _exportTablItemeSerializer.ReadTArrayToList(stream, package.ExportTable, package.Header.ExportCount);

        return package;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, UnrealPackage value)
    {
        throw new NotImplementedException();
    }
}