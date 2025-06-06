﻿using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

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
        _componentSerializer.SerializeObject(obj, objectStream);

        objectStream.WriteTArray(obj.FStaticMeshComponentLodInfos, (stream, info) => _staticMeshComponentSerializer.SerializeObject(info, stream));
    }
}