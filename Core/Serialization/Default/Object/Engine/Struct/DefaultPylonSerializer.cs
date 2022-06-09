using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultPylonSerializer : BaseObjectSerializer<APylon>
{
    private readonly IStreamSerializerFor<ObjectIndex> _objecIndexSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultPylonSerializer(IStreamSerializerFor<ObjectIndex> objecIndexSerializer, IObjectSerializer<UObject> objectSerializer)
    {
        _objecIndexSerializer = objecIndexSerializer;
        _objectSerializer = objectSerializer;
    }

    public override void DeserializeObject(APylon obj, Stream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        obj.NavMeshPtr = obj.OwnerPackage.GetObject(_objecIndexSerializer.Deserialize(objectStream));
        obj.ObstacleMesh = obj.OwnerPackage.GetObject(_objecIndexSerializer.Deserialize(objectStream));
    }

    public override void SerializeObject(APylon obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}