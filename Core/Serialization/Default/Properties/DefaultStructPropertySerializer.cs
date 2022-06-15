﻿using Core.Classes;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Properties;

public class DefaultStructPropertySerializer : BaseObjectSerializer<UStructProperty>
{
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public DefaultStructPropertySerializer(IObjectSerializer<UProperty> propertySerializer, IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        _propertySerializer = propertySerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UStructProperty obj, IUnrealPackageStream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.Struct = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream)) as UScriptStruct;
    }

    /// <inheritdoc />
    public override void SerializeObject(UStructProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}