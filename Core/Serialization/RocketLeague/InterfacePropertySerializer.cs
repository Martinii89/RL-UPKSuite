﻿using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;

namespace Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class InterfacePropertySerializer : BaseObjectSerializer<UInterfaceProperty>
{
    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public InterfacePropertySerializer(IObjectSerializer<UProperty> propertySerializer)
    {
        _propertySerializer = propertySerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UInterfaceProperty obj, IUnrealPackageStream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.InterfaceClass = objectStream.ReadObject() as UClass;
        var name = objectStream.ReadFNameStr();
        if (name != "None")
        {
            Debugger.Break();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UInterfaceProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}