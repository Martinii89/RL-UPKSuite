using Core.Classes;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultUDominantSpotLightComponentSerializer : BaseObjectSerializer<UDominantSpotLightComponent>
{
    private readonly IObjectSerializer<UComponent> _componentSerializer;

    public DefaultUDominantSpotLightComponentSerializer(IObjectSerializer<UComponent> componentSerializer)
    {
        _componentSerializer = componentSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UDominantSpotLightComponent obj, IUnrealPackageStream objectStream)
    {
        var dominantLightShadowMapCount = objectStream.ReadInt32();

        for (var i = 0; i < dominantLightShadowMapCount; i++)
        {
            obj.DominantLightShadowMap.AddRange(objectStream.ReadUInt16s(dominantLightShadowMapCount));
        }

        _componentSerializer.DeserializeObject(obj, objectStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(UDominantSpotLightComponent obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}