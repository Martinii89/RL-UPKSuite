using Core.Classes;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultStaticMeshComponentSerializer : BaseObjectSerializer<UStaticMeshComponent>
{
    private readonly IObjectSerializer<UComponent> _componentSerializer;
    private readonly IObjectSerializer<FStaticMeshComponentLODInfo> _staticMeshComponentSerializer;

    public DefaultStaticMeshComponentSerializer(IObjectSerializer<UComponent> componentSerializer,
        IObjectSerializer<FStaticMeshComponentLODInfo> staticMeshComponentSerializer)
    {
        _componentSerializer = componentSerializer;
        _staticMeshComponentSerializer = staticMeshComponentSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UStaticMeshComponent obj, IUnrealPackageStream objectStream)
    {
        _componentSerializer.DeserializeObject(obj, objectStream);

        obj.FStaticMeshComponentLodInfos = _staticMeshComponentSerializer.ReadTArrayToList(objectStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(UStaticMeshComponent obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}