using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultSkeletalMeshSerializer : BaseObjectSerializer<USkeletalMesh>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultSkeletalMeshSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }

    public override void DeserializeObject(USkeletalMesh obj, Stream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
    }

    public override void SerializeObject(USkeletalMesh obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}