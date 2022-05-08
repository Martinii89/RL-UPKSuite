using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default;

/// <summary>
///     Serializer for the items in the import table
/// </summary>
public class ImportTableItemSerializer : IStreamSerializerFor<ImportTableItem>
{
    private readonly IStreamSerializerFor<FName> _nameSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;

    /// <summary>
    ///     The serializers requires a name and object reference serializer
    /// </summary>
    /// <param name="nameSerializer"></param>
    /// <param name="objectIndexSerializer"></param>
    public ImportTableItemSerializer(IStreamSerializerFor<FName> nameSerializer, IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    {
        _nameSerializer = nameSerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public ImportTableItem Deserialize(Stream stream)
    {
        return new ImportTableItem
        {
            ClassPackage = _nameSerializer.Deserialize(stream),
            ClassName = _nameSerializer.Deserialize(stream),
            OuterIndex = _objectIndexSerializer.Deserialize(stream),
            ObjectName = _nameSerializer.Deserialize(stream)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, ImportTableItem value)
    {
        _nameSerializer.Serialize(stream, value.ClassPackage);
        _nameSerializer.Serialize(stream, value.ClassName);
        _objectIndexSerializer.Serialize(stream, value.OuterIndex);
        _nameSerializer.Serialize(stream, value.ObjectName);
    }
}