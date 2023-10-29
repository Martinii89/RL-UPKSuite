using System.Diagnostics;
using Core.Classes;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultBrushComponentSerializer : BaseObjectSerializer<UBrushComponent>
{
    private readonly IObjectSerializer<UComponent> _componentSerializer;

    public DefaultBrushComponentSerializer(IObjectSerializer<UComponent> componentSerializer)
    {
        _componentSerializer = componentSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UBrushComponent obj, IUnrealPackageStream objectStream)
    {
        _componentSerializer.DeserializeObject(obj, objectStream);

        obj.CachedPhysBrushDataCount = objectStream.ReadInt32();
        if (obj.CachedPhysBrushDataCount > 0)
        {
            Debugger.Break();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UBrushComponent obj, IUnrealPackageStream objectStream)
    {
        _componentSerializer.SerializeObject(obj, objectStream);
        objectStream.WriteInt32(0);
    }
}