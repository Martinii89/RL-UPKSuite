using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Engine.Struct;

public class DefaultPylonSerializer : BaseObjectSerializer<APylon>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultPylonSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(APylon obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        obj.NavMeshPtr = objectStream.ReadObject();
        obj.ObstacleMesh = objectStream.ReadObject();
    }

    /// <inheritdoc />
    public override void SerializeObject(APylon obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        objectStream.WriteObject(obj.NavMeshPtr);
        objectStream.WriteObject(obj.ObstacleMesh);
    }
}