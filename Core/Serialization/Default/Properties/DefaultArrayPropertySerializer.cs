using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Properties;

public class DefaultArrayPropertySerializer : BaseObjectSerializer<UArrayProperty>
{
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public DefaultArrayPropertySerializer(IObjectSerializer<UProperty> propertySerializer, IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        _propertySerializer = propertySerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UArrayProperty obj, Stream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.InnerProperty = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) as UProperty;
    }

    /// <inheritdoc />
    public override void SerializeObject(UArrayProperty obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}