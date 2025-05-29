using RlUpk.Core.Classes.Core;
using RlUpk.Core.Flags;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object;

public class DefaultFunctionSerializer : BaseObjectSerializer<UFunction>
{
    private readonly IObjectSerializer<UStruct> _structSerializer;

    public DefaultFunctionSerializer(IObjectSerializer<UStruct> structSerializer)
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