using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

public class DefaultDominantDirectionalLightComponentSerializer : BaseObjectSerializer<UDominantDirectionalLightComponent>
{
    private readonly IObjectSerializer<UComponent> _componentSerializer;

    public DefaultDominantDirectionalLightComponentSerializer(IObjectSerializer<UComponent> componentSerializer)
    {
        _componentSerializer = componentSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UDominantDirectionalLightComponent obj, IUnrealPackageStream objectStream)
    {
        obj.DominantLightShadowMap = objectStream.ReadTArray(stream => stream.ReadUInt16());
        _componentSerializer.DeserializeObject(obj, objectStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(UDominantDirectionalLightComponent obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}