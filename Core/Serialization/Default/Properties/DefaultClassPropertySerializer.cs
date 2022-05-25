﻿using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Properties;

public class DefaultClassPropertySerializer : BaseObjectSerializer<UClassProperty>
{
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UObjectProperty> _objectPropertySerializer;

    public DefaultClassPropertySerializer(IObjectSerializer<UObjectProperty> objectPropertySerializer, IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    {
        _objectPropertySerializer = objectPropertySerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UClassProperty obj, Stream objectStream)
    {
        _objectPropertySerializer.DeserializeObject(obj, objectStream);

        obj.MetaClass = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) as UClass;
    }

    /// <inheritdoc />
    public override void SerializeObject(UClassProperty obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}