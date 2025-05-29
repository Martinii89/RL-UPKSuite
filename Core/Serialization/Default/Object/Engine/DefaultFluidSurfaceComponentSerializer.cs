using System.Diagnostics;

using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

public class DefaultFluidSurfaceComponentSerializer : BaseObjectSerializer<UFluidSurfaceComponent>
{
    private readonly IObjectSerializer<UComponent> _componentSerializer;

    public DefaultFluidSurfaceComponentSerializer(IObjectSerializer<UComponent> componentSerializer)
    {
        _componentSerializer = componentSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UFluidSurfaceComponent obj, IUnrealPackageStream objectStream)
    {
        _componentSerializer.DeserializeObject(obj, objectStream);

        var LightMapCount = objectStream.ReadInt32();
        if (LightMapCount > 0)
        {
            Debugger.Break();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UFluidSurfaceComponent obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}