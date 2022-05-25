using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Properties;

public class DefaultMapPropertySerializer : BaseObjectSerializer<UMapProperty>
{
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public DefaultMapPropertySerializer(IObjectSerializer<UProperty> propertySerializer, IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    {
        _propertySerializer = propertySerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UMapProperty obj, Stream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.Key = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) as UProperty;
        obj.Value = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) as UProperty;
    }

    /// <inheritdoc />
    public override void SerializeObject(UMapProperty obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}