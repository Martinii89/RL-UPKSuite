using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultPylonSerializer : BaseObjectSerializer<APylon>
{
    private readonly IStreamSerializer<ObjectIndex> _objecIndexSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultPylonSerializer(IStreamSerializer<ObjectIndex> objecIndexSerializer, IObjectSerializer<UObject> objectSerializer)
    {
        _objecIndexSerializer = objecIndexSerializer;
        _objectSerializer = objectSerializer;
    }

    public override void DeserializeObject(APylon obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        obj.NavMeshPtr = obj.OwnerPackage.GetObject(_objecIndexSerializer.Deserialize(objectStream.BaseStream));
        obj.ObstacleMesh = obj.OwnerPackage.GetObject(_objecIndexSerializer.Deserialize(objectStream.BaseStream));
    }

    public override void SerializeObject(APylon obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}