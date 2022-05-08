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
        var names = _nameTableItemSerializer.ReadTArray(stream, package.Header.NameCount);
        package.NameTable.AddRange(names);

        stream.Seek(package.Header.ImportOffset, SeekOrigin.Begin);
        var imports = _importTableItemSerializer.ReadTArray(stream, package.Header.ImportCount);
        package.ImportTable.AddRange(imports);

        stream.Seek(package.Header.ExportOffset, SeekOrigin.Begin);
        var exports = _exportTablItemeSerializer.ReadTArray(stream, package.Header.ExportCount);
        package.ExportTable.AddRange(exports);


        return package;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, UnrealPackage value)
    {
        throw new NotImplementedException();
    }
}