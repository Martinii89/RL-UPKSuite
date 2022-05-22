using Core.Classes;
using Core.Flags;
using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Serialization.Default.Object;

public class DefaultFunctionSerializer : BaseObjectSerializer<UFunction>
{
    private readonly IStreamSerializerFor<FName> _fnameSerializer;
    private readonly IObjectSerializer<UStruct> _structSerializer;

    public DefaultFunctionSerializer(IObjectSerializer<UStruct> structSerializer, IStreamSerializerFor<FName> fnameSerializer)
    {
        _structSerializer = structSerializer;
        _fnameSerializer = fnameSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UFunction obj, Stream objectStream)
    {
        _structSerializer.DeserializeObject(obj, objectStream);
        obj.INative = objectStream.ReadUInt16();
        obj.OperPrecedence = (byte) objectStream.ReadByte();
        obj.FunctionFlags = objectStream.ReadUInt32();
        objectStream.Move(4);

        if (obj.HasFunctionFlag(FunctionFlags.Net))
        {
            obj.RepOffset = objectStream.ReadUInt16();
        }

        //objectStream.Move(2);
        //var currentOffset = objectStream.Position - obj.ExportTableItem!.SerialOffset;
        //var friendlyNamePos = obj.ExportTableItem.SerialSize - 12;
        //objectStream.Move(friendlyNamePos - currentOffset);

        obj.FriendlyName = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream));
    }

    /// <inheritdoc />
    public override void SerializeObject(UFunction obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}