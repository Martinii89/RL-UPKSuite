using Core.Classes.Engine.Structs;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class MipSerializer : IStreamSerializerFor<Mip>
{
    private readonly IStreamSerializerFor<FByteBulkData> _bulkDataSerializer;

    public MipSerializer(IStreamSerializerFor<FByteBulkData> bulkDataSerializer)
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
        throw new NotImplementedException();
    }
}