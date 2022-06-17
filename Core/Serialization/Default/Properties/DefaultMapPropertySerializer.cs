using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Properties;

public class DefaultMapPropertySerializer : BaseObjectSerializer<UMapProperty>
{
    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public DefaultMapPropertySerializer(IObjectSerializer<UProperty> propertySerializer)
    {
        _propertySerializer = propertySerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UMapProperty obj, IUnrealPackageStream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.Key = objectStream.ReadObject() as UProperty;
        obj.Value = objectStream.ReadObject() as UProperty;
    }

    /// <inheritdoc />
    public override void SerializeObject(UMapProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}