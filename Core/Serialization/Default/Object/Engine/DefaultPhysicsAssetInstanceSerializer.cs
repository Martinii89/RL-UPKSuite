using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using StreamExtensions = Core.Serialization.Extensions.StreamExtensions;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultPhysicsAssetInstanceSerializer : BaseObjectSerializer<UPhysicsAssetInstance>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultPhysicsAssetInstanceSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UPhysicsAssetInstance obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);

        obj.CollisionDisableTable = StreamExtensions.ReadDictionary(objectStream.BaseStream,
            stream => new FRigidBodyIndexPair { Index1 = stream.ReadInt32(), Index2 = stream.ReadInt32() },
            stream => stream.ReadByte() == 0);
    }

    /// <inheritdoc />
    public override void SerializeObject(UPhysicsAssetInstance obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}