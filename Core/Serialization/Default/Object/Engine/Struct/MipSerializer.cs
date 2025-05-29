using RlUpk.Core.Classes.Engine.Structs;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Engine.Struct;

public class MipSerializer : IStreamSerializer<Mip>
{
    private readonly IStreamSerializer<FByteBulkData> _bulkDataSerializer;

    public MipSerializer(IStreamSerializer<FByteBulkData> bulkDataSerializer)
    {
        _bulkDataSerializer = bulkDataSerializer;
    }

    public Mip Deserialize(Stream stream)
    {
        return new Mip
        {
            Data = _bulkDataSerializer.Deserialize(stream),
            SizeX = stream.ReadInt32(),
            SizeY = stream.ReadInt32()
        };
    }

    public void Serialize(Stream stream, Mip value)
    {
        _bulkDataSerializer.Serialize(stream, value.Data);
        stream.WriteInt32(value.SizeX);
        stream.WriteInt32(value.SizeY);
    }
}