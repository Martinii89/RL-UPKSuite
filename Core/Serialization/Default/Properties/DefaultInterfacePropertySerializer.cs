﻿using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Properties;

public class DefaultInterfacePropertySerializer : BaseObjectSerializer<UInterfaceProperty>
{
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public DefaultInterfacePropertySerializer(IObjectSerializer<UProperty> propertySerializer,
        IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    {
        _propertySerializer = propertySerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UInterfaceProperty obj, Stream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.InterfaceClass = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) as UClass;
    }

    /// <inheritdoc />
    public override void SerializeObject(UInterfaceProperty obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}