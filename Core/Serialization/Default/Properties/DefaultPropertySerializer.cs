﻿using Core.Classes;
using Core.Classes.Core.Properties;
using Core.Flags;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Properties;

public class DefaultPropertySerializer : BaseObjectSerializer<UProperty>
{
    private readonly IObjectSerializer<UField> _fieldSerializer;
    private readonly IStreamSerializerFor<FName> _fnameSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _ObjectIndexSerializer;

    public DefaultPropertySerializer(IObjectSerializer<UField> fieldSerializer, IStreamSerializerFor<FName> fnameSerializer,
        IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    {
        _fieldSerializer = fieldSerializer;
        _fnameSerializer = fnameSerializer;
        _ObjectIndexSerializer = objectIndexSerializer;
    }

    public override void DeserializeObject(UProperty obj, Stream objectStream)
    {
        _fieldSerializer.DeserializeObject(obj, objectStream);

        obj.ArrayDim = objectStream.ReadInt32();
        obj.PropertyFlags = objectStream.ReadUInt64();
        obj.Category = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream));
        obj.ArraySizeEnum = obj.OwnerPackage.GetObject(_ObjectIndexSerializer.Deserialize(objectStream)) as UEnum;

        if (obj.HasPropertyFlag(PropertyFlagsLO.Net))
        {
            obj.RepOffset = objectStream.ReadUInt16();
        }
    }

    public override void SerializeObject(UProperty obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}