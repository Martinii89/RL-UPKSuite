﻿using Core.Classes;
using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Serialization.Default.Object;

public class DefaultEnumSerializer : BaseObjectSerializer<UEnum>
{
    private readonly IObjectSerializer<UField> _fieldSerializer;
    private readonly IStreamSerializer<FName> _fnameSerializer;

    public DefaultEnumSerializer(IObjectSerializer<UField> fieldSerializer, IStreamSerializer<FName> fnameSerializer)
    {
        _fieldSerializer = fieldSerializer;
        _fnameSerializer = fnameSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UEnum obj, IUnrealPackageStream objectStream)
    {
        _fieldSerializer.DeserializeObject(obj, objectStream);
        var count = objectStream.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            obj.Names.Add(obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream.BaseStream)));
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UEnum obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}