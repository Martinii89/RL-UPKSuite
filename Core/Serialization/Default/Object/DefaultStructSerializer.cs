using Core.Classes;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object;

/// <inheritdoc />
public class DefaultStructSerializer : IObjectSerializer<UStruct>
{
    private readonly IObjectSerializer<UField> _fieldSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerialiser;

    public DefaultStructSerializer(IObjectSerializer<UField> fieldSerializer, IStreamSerializerFor<ObjectIndex> objectIndexSerialiser)
    {
        _fieldSerializer = fieldSerializer;
        _objectIndexSerialiser = objectIndexSerialiser;
    }

    /// <inheritdoc />
    public void DeserializeObject(UStruct obj, Stream objectStream)
    {
        _fieldSerializer.DeserializeObject(obj, objectStream);

        obj.SuperStruct = obj.OwnerPackage.GetObject(_objectIndexSerialiser.Deserialize(objectStream)) as UStruct;

        obj.Children = obj.OwnerPackage.GetObject(_objectIndexSerialiser.Deserialize(objectStream)) as UField;
    }

    /// <inheritdoc />
    public void SerializeObject(UStruct obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}