using System.Diagnostics;
using Core.Classes;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultStaticMeshComponentSerializer : BaseObjectSerializer<UStaticMeshComponent>
{
    private readonly IObjectSerializer<UComponent> _componentSerializer;

    public DefaultStaticMeshComponentSerializer(IObjectSerializer<UComponent> componentSerializer)
    {
        _componentSerializer = componentSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UStaticMeshComponent obj, Stream objectStream)
    {
        _componentSerializer.DeserializeObject(obj, objectStream);

        var lodDataCount = objectStream.ReadInt32();
        if (lodDataCount > 0)
        {
            Debugger.Break();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UStaticMeshComponent obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}