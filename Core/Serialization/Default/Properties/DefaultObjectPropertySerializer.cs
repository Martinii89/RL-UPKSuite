using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Properties;

public class DefaultObjectPropertySerializer : BaseObjectSerializer<UObjectProperty>
{
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public DefaultObjectPropertySerializer(IObjectSerializer<UProperty> propertySerializer, IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        _propertySerializer = propertySerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UObjectProperty obj, Stream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.PropertyClass = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) as UClass;
    }

    /// <inheritdoc />
    public override void SerializeObject(UObjectProperty obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}