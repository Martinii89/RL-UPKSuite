﻿using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Properties;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Properties;

public class DefaultInterfacePropertySerializer : BaseObjectSerializer<UInterfaceProperty>
{
    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public DefaultInterfacePropertySerializer(IObjectSerializer<UProperty> propertySerializer)
    {
        _propertySerializer = propertySerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UInterfaceProperty obj, IUnrealPackageStream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.InterfaceClass = objectStream.ReadObject() as UClass;
    }

    /// <inheritdoc />
    public override void SerializeObject(UInterfaceProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}