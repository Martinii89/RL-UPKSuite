using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default;

/// <summary>
///     Serializes the UnrealPackage data. It does not link up or create any objects, just the header and table data is
///     serialized.
/// </summary>
public class UnrealPackageSerializer : IStreamSerializerFor<UnrealPackage>
{
    private readonly IStreamSerializerFor<ExportTableItem> _exportTablItemeSerializer;
    private readonly IStreamSerializerFor<FileSummary> _fileSummarySerializerFor;
    private readonly IStreamSerializerFor<ImportTableItem> _importTableItemSerializer;
    private readonly IStreamSerializerFor<NameTableItem> _nameTableItemSerializer;

    /// <summary>
    ///     Constructs a serializer capable of serializing a UnrealPackage by giving it all the required sub serializers
    /// </summary>
    /// <param name="fileSummarySerializerFor"></param>
    /// <param name="nameTableItemSerializer"></param>
    /// <param name="importTableItemSerializer"></param>
    /// <param name="exportTablItemeSerializer"></param>
    public UnrealPackageSerializer(IStreamSerializerFor<FileSummary> fileSummarySerializerFor, IStreamSerializerFor<NameTableItem> nameTableItemSerializer,
        IStreamSerializerFor<ImportTableItem> importTableItemSerializer, IStreamSerializerFor<ExportTableItem> exportTablItemeSerializer)
    {
        _fileSummarySerializerFor = fileSummarySerializerFor;
        _nameTableItemSerializer = nameTableItemSerializer;
        _importTableItemSerializer = importTableItemSerializer;
        _exportTablItemeSerializer = exportTablItemeSerializer;
    }

    /// <inheritdoc />
    public UnrealPackage Deserialize(Stream stream)
    {
        var package = new UnrealPackage();
        package.Header = _fileSummarySerializerFor.Deserialize(stream);

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