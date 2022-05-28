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
    public override void DeserializeObject(UBrushComponent obj, Stream objectStream)
    {
        _componentSerializer.DeserializeObject(obj, objectStream);

        var cachedPhysBrushDataCount = objectStream.ReadInt32();
        if (cachedPhysBrushDataCount > 0)
        {
            Debugger.Break();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UBrushComponent obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}