using System.Diagnostics;
using Core.Classes;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultUSpeedTreeComponentSerializer : BaseObjectSerializer<USpeedTreeComponent>
{
    private readonly IObjectSerializer<UComponent> _componentSerializer;

    public DefaultUSpeedTreeComponentSerializer(IObjectSerializer<UComponent> componentSerializer)
    {
        _componentSerializer = componentSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(USpeedTreeComponent obj, IUnrealPackageStream objectStream)
    {
        _componentSerializer.DeserializeObject(obj, objectStream);

        var BranchLightMapCount = objectStream.ReadInt32();
        var FrondLightMapCount = objectStream.ReadInt32();
        var LeafCardLightMapCount = objectStream.ReadInt32();
        var BillboardLightMapCount = objectStream.ReadInt32();
        var LeafMeshLightMapCount = objectStream.ReadInt32();
        if (BranchLightMapCount > 0 || FrondLightMapCount > 0 || LeafCardLightMapCount > 0 || BillboardLightMapCount > 0 || LeafMeshLightMapCount > 0)
        {
            Debugger.Break();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(USpeedTreeComponent obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}