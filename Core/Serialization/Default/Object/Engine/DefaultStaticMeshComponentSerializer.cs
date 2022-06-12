using Core.Classes;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultStaticMeshComponentSerializer : BaseObjectSerializer<UStaticMeshComponent>
{
    private readonly IObjectSerializer<UComponent> _componentSerializer;
    private readonly IStreamSerializerFor<FStaticMeshComponentLODInfo> _staticMeshComponentSerializer;

    public DefaultStaticMeshComponentSerializer(IObjectSerializer<UComponent> componentSerializer,
        IStreamSerializerFor<FStaticMeshComponentLODInfo> staticMeshComponentSerializer)
    {
        _componentSerializer = componentSerializer;
        _staticMeshComponentSerializer = staticMeshComponentSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UStaticMeshComponent obj, Stream objectStream)
    {
        _componentSerializer.DeserializeObject(obj, objectStream);

        obj.FStaticMeshComponentLodInfos = _staticMeshComponentSerializer.ReadTArrayToList(objectStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(UStaticMeshComponent obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}