﻿using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class InterfacePropertySerializer : BaseObjectSerializer<UInterfaceProperty>
{
    private readonly IStreamSerializerFor<FName> _fnameSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public InterfacePropertySerializer(IObjectSerializer<UProperty> propertySerializer,
        IStreamSerializerFor<ObjectIndex> objectIndexSerializer, IStreamSerializerFor<FName> fnameSerializer)
    {
        _propertySerializer = propertySerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _fnameSerializer = fnameSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UInterfaceProperty obj, Stream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.InterfaceClass = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) as UClass;
        var dummyFNameMaybe = _fnameSerializer.Deserialize(objectStream);
        var name = obj.OwnerPackage.GetName(dummyFNameMaybe);
        if (name != "None")
        {
            Debugger.Break();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UInterfaceProperty obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}