using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultMaterialInstanceSerializer : BaseObjectSerializer<UMaterialInstance>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultMaterialInstanceSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }

    public override void DeserializeObject(UMaterialInstance obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        DropRamainingNativeData(obj, objectStream.BaseStream);
    }

    public override void SerializeObject(UMaterialInstance obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}