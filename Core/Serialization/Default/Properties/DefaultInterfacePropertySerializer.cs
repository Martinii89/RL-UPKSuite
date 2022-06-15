using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Properties;

public class DefaultInterfacePropertySerializer : BaseObjectSerializer<UInterfaceProperty>
{
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public DefaultInterfacePropertySerializer(IObjectSerializer<UProperty> propertySerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        _propertySerializer = propertySerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UInterfaceProperty obj, IUnrealPackageStream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.InterfaceClass = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream)) as UClass;
    }

    /// <inheritdoc />
    public override void SerializeObject(UInterfaceProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}