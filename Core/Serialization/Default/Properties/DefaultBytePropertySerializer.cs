using Core.Classes;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Properties;

public class DefaultBytePropertySerializer : BaseObjectSerializer<UByteProperty>
{
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public DefaultBytePropertySerializer(IObjectSerializer<UProperty> propertySerializer, IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    {
        _propertySerializer = propertySerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UByteProperty obj, Stream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.Enum = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) as UEnum;
    }

    /// <inheritdoc />
    public override void SerializeObject(UByteProperty obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}