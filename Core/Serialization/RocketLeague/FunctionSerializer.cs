﻿using Core.Classes;
using Core.Flags;
using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class FunctionSerializer : BaseObjectSerializer<UFunction>
{
    private readonly IStreamSerializer<FName> _fnameSerializer;
    private readonly IObjectSerializer<UStruct> _structSerializer;

    public FunctionSerializer(IObjectSerializer<UStruct> structSerializer, IStreamSerializer<FName> fnameSerializer)
    {
        _structSerializer = structSerializer;
        _fnameSerializer = fnameSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UFunction obj, IUnrealPackageStream objectStream)
    {
        _structSerializer.DeserializeObject(obj, objectStream);
        obj.INative = objectStream.ReadUInt16();
        obj.OperPrecedence = objectStream.ReadByte();
        obj.FunctionFlags = objectStream.ReadUInt32();
        objectStream.BaseStream.Move(4); // RL specific (flags probably 64 bit now)

        if (obj.HasFunctionFlag(FunctionFlags.Net))
        {
            obj.RepOffset = objectStream.ReadUInt16();
        }

        obj.FriendlyName = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream.BaseStream));
    }

    /// <inheritdoc />
    public override void SerializeObject(UFunction obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}