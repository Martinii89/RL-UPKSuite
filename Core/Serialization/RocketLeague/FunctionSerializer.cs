using Core.Classes;
using Core.Flags;
using Core.Serialization.Abstraction;

namespace Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class FunctionSerializer : BaseObjectSerializer<UFunction>
{
    private readonly IObjectSerializer<UStruct> _structSerializer;

    public FunctionSerializer(IObjectSerializer<UStruct> structSerializer)
    {
        _structSerializer = structSerializer;
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

        obj.FriendlyName = objectStream.ReadFNameStr();
    }

    /// <inheritdoc />
    public override void SerializeObject(UFunction obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}