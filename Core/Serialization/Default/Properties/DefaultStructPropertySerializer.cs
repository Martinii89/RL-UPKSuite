using Core.Classes;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Properties;

public class DefaultStructPropertySerializer : BaseObjectSerializer<UStructProperty>
{
    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public DefaultStructPropertySerializer(IObjectSerializer<UProperty> propertySerializer)
    {
        _propertySerializer = propertySerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UStructProperty obj, IUnrealPackageStream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);
        obj.Struct = objectStream.ReadObject() as UScriptStruct;
    }

    /// <inheritdoc />
    public override void SerializeObject(UStructProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}