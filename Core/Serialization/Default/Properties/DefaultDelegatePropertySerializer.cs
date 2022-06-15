using Core.Classes;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Properties;

public class DefaultDelegatePropertySerializer : BaseObjectSerializer<UDelegateProperty>
{
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UObjectProperty> _propertySerializer;

    public DefaultDelegatePropertySerializer(IObjectSerializer<UProperty> propertySerializer, IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        _propertySerializer = propertySerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UDelegateProperty obj, Stream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.FunctionObject = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) as UFunction;
        obj.DelegateObject = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
    }

    /// <inheritdoc />
    public override void SerializeObject(UDelegateProperty obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}