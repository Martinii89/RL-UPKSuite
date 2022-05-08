using Core.Types.FileSummeryInner;

namespace Core.Serialization.Default;

internal class FCompressedChunkHeaderSerializer : IStreamSerializerFor<FCompressedChunkHeader>
{
    private readonly IStreamSerializerFor<FCompressedChunkBlock> _blockSerializer;

    public FCompressedChunkHeaderSerializer(IStreamSerializerFor<FCompressedChunkBlock> blockSerializer)
    {
        _blockSerializer = blockSerializer;
    }

    public FCompressedChunkHeader Deserialize(Stream stream)
    {
        return new FCompressedChunkHeader
        {
            Tag = stream.ReadInt32(),
            BlockSize = stream.ReadInt32(),
            Summary = _blockSerializer.Deserialize(stream)
        };
    }

    public void Serialize(Stream stream, FCompressedChunkHeader value)
    {
        throw new NotImplementedException();
    }
}

internal class FCompressedChunkBlockSerializer : IStreamSerializerFor<FCompressedChunkBlock>
{
    public FCompressedChunkBlock Deserialize(Stream stream)
    {
        return new FCompressedChunkBlock
        {
            CompressedSize = stream.ReadInt32(),
            UncompressedSize = stream.ReadInt32()
        };
    }

    public void Serialize(Stream stream, FCompressedChunkBlock value)
    {
        throw new NotImplementedException();
    }
}