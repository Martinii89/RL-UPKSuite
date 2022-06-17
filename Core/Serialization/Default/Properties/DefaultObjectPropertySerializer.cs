using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Properties;

public class DefaultObjectPropertySerializer : BaseObjectSerializer<UObjectProperty>
{
    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public DefaultObjectPropertySerializer(IObjectSerializer<UProperty> propertySerializer)
    {
        _propertySerializer = propertySerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UObjectProperty obj, IUnrealPackageStream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.PropertyClass = objectStream.ReadObject() as UClass;
    }

    /// <inheritdoc />
    public override void SerializeObject(UObjectProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}