﻿using System.Diagnostics;
using Core.Classes;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultDecalComponentSerializer : BaseObjectSerializer<UDecalComponent>
{
    private readonly IObjectSerializer<UComponent> _componentSerializer;

    public DefaultDecalComponentSerializer(IObjectSerializer<UComponent> componentSerializer)
    {
        _componentSerializer = componentSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UDecalComponent obj, IUnrealPackageStream objectStream)
    {
        _componentSerializer.DeserializeObject(obj, objectStream);

        var NumStaticReceivers = objectStream.ReadInt32();
        if (NumStaticReceivers > 0)
        {
            Debugger.Break();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UDecalComponent obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}