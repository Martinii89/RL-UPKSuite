using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Classes.Engine.Structs;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

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

        obj.CollisionDisableTable = objectStream.ReadDictionary(stream => new FRigidBodyIndexPair { Index1 = stream.ReadInt32(), Index2 = stream.ReadInt32() },
            stream => stream.ReadBool());
    }

    /// <inheritdoc />
    public override void SerializeObject(UPhysicsAssetInstance obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        objectStream.WriteDictionary(obj.CollisionDisableTable, (stream, pair) =>
        {
            stream.WriteInt32(pair.Index1);
            stream.WriteInt32(pair.Index2);
        }, (stream, b) => stream.WriteBool(b));
    }
}