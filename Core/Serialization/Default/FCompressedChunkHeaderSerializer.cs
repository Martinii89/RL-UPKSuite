using Core.Types.FileSummeryInner;

namespace Core.Serialization.Default;

internal class FCompressedChunkHeaderSerializer : IStreamSerializer<FCompressedChunkHeader>
{
    private readonly IStreamSerializer<FCompressedChunkBlock> _blockSerializer;

    public FCompressedChunkHeaderSerializer(IStreamSerializer<FCompressedChunkBlock> blockSerializer)
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

internal class FCompressedChunkBlockSerializer : IStreamSerializer<FCompressedChunkBlock>
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