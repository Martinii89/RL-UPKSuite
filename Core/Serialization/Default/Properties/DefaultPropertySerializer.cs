﻿using Core.Classes;
using Core.Classes.Core.Properties;
using Core.Flags;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Properties;

public class DefaultPropertySerializer : BaseObjectSerializer<UProperty>
{
    private readonly IObjectSerializer<UField> _fieldSerializer;

    public DefaultPropertySerializer(IObjectSerializer<UField> fieldSerializer)
    {
        _fieldSerializer = fieldSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UProperty obj, IUnrealPackageStream objectStream)
    {
        _fieldSerializer.DeserializeObject(obj, objectStream);

        obj.ArrayDim = objectStream.ReadInt32();
        obj.PropertyFlags = objectStream.ReadUInt64();
        obj.Category = objectStream.ReadFNameStr();
        obj.ArraySizeEnum = objectStream.ReadObject() as UEnum;

        if (obj.HasPropertyFlag(PropertyFlagsLO.Net))
        {
            obj.RepOffset = objectStream.ReadUInt16();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}