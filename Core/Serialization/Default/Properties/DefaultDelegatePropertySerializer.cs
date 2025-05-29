using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Properties;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Properties;

public class DefaultDelegatePropertySerializer : BaseObjectSerializer<UDelegateProperty>
{
    private readonly IObjectSerializer<UObjectProperty> _propertySerializer;

    public DefaultDelegatePropertySerializer(IObjectSerializer<UProperty> propertySerializer)
    {
        _propertySerializer = propertySerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UDelegateProperty obj, IUnrealPackageStream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.FunctionObject = objectStream.ReadObject() as UFunction;
        obj.DelegateObject = objectStream.ReadObject();
    }

    /// <inheritdoc />
    public override void SerializeObject(UDelegateProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}