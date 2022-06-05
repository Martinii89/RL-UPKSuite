using Core.Classes;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultDominantDirectionalLightComponentSerializer : BaseObjectSerializer<UDominantDirectionalLightComponent>
{
    private readonly IObjectSerializer<UComponent> _componentSerializer;

    public DefaultDominantDirectionalLightComponentSerializer(IObjectSerializer<UComponent> componentSerializer)
    {
        _componentSerializer = componentSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UDominantDirectionalLightComponent obj, Stream objectStream)
    {
        obj.DominantLightShadowMap = objectStream.ReadTarray(stream => stream.ReadUInt16());
        _componentSerializer.DeserializeObject(obj, objectStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(UDominantDirectionalLightComponent obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}