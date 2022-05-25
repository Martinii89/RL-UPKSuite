using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Properties;

public class DefaultObjectPropertySerializer : BaseObjectSerializer<UObjectProperty>
{
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public DefaultObjectPropertySerializer(IObjectSerializer<UProperty> propertySerializer, IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    {
        _propertySerializer = propertySerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UObjectProperty obj, Stream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.Object = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
    }

    /// <inheritdoc />
    public override void SerializeObject(UObjectProperty obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}