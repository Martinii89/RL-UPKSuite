﻿using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Properties;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types.PackageTables;

namespace RlUpk.Core.Serialization.Default.Properties;

public class DefaultBytePropertySerializer : BaseObjectSerializer<UByteProperty>
{
    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public DefaultBytePropertySerializer(IObjectSerializer<UProperty> propertySerializer, IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        _propertySerializer = propertySerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UByteProperty obj, IUnrealPackageStream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.Enum = objectStream.ReadObject() as UEnum;
    }

    /// <inheritdoc />
    public override void SerializeObject(UByteProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}