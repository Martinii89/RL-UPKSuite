using Core.Classes;
using Core.Classes.Core;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object;

public class DefaultFieldSerializer : IObjectSerializer<UField>
{
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerialiser;

    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultFieldSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializerFor<ObjectIndex> objectIndexSerialiser)
    {
        _objectSerializer = objectSerializer;
        _objectIndexSerialiser = objectIndexSerialiser;
    }

    public void DeserializeObject(UField field, Stream objectStream)
    {
        _objectSerializer.DeserializeObject(field, objectStream);

        field.Next = field.OwnerPackage.GetObject(_objectIndexSerialiser.Deserialize(objectStream));
    }

    public void SerializeObject(UField obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}