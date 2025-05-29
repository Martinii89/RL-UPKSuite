using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Classes.Engine.Structs;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

public class DefaultRB_BodySetupSerializer : BaseObjectSerializer<URB_BodySetup>
{
    private readonly IStreamSerializer<FKCachedConvexData> _kCachedConvexDataSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultRB_BodySetupSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializer<FKCachedConvexData> kCachedConvexDataSerializer)
    {
        _objectSerializer = objectSerializer;
        _kCachedConvexDataSerializer = kCachedConvexDataSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(URB_BodySetup obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);


        obj.PreCachedPhysData = _kCachedConvexDataSerializer.ReadTArrayToList(objectStream.BaseStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(URB_BodySetup obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        objectStream.WriteTArray(obj.PreCachedPhysData, _kCachedConvexDataSerializer);
    }
}