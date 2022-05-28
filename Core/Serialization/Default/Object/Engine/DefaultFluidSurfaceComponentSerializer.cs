using System.Diagnostics;
using Core.Classes;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultFluidSurfaceComponentSerializer : BaseObjectSerializer<UFluidSurfaceComponent>
{
    private readonly IObjectSerializer<UComponent> _componentSerializer;

    public DefaultFluidSurfaceComponentSerializer(IObjectSerializer<UComponent> componentSerializer)
    {
        _componentSerializer = componentSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UFluidSurfaceComponent obj, Stream objectStream)
    {
        _componentSerializer.DeserializeObject(obj, objectStream);

        var LightMapCount = objectStream.ReadInt32();
        if (LightMapCount > 0)
        {
            Debugger.Break();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UFluidSurfaceComponent obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}