using RlUpk.Core.Classes.Core.Properties;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Properties;

public class DefaultArrayPropertySerializer : BaseObjectSerializer<UArrayProperty>
{
    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public DefaultArrayPropertySerializer(IObjectSerializer<UProperty> propertySerializer)
    {
        _propertySerializer = propertySerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UArrayProperty obj, IUnrealPackageStream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.InnerProperty = objectStream.ReadObject() as UProperty;
    }

    /// <inheritdoc />
    public override void SerializeObject(UArrayProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}