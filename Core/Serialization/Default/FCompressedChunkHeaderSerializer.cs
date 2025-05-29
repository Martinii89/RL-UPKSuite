using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types.FileSummeryInner;

namespace RlUpk.Core.Serialization.Default;

internal class FCompressedChunkHeaderSerializer : IStreamSerializer<FCompressedChunkHeader>
{
    private readonly IStreamSerializer<FCompressedChunkInfo> _blockSerializer;

    public FCompressedChunkHeaderSerializer(IStreamSerializer<FCompressedChunkInfo> blockSerializer)
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

public class FCompressedChunkinfoSerializer : IStreamSerializer<FCompressedChunkInfo>
{
    /// <inheritdoc />
    public FCompressedChunkInfo Deserialize(Stream stream)
    {
        return new FCompressedChunkInfo
        {
            CompressedSize = stream.ReadInt32(),
            UncompressedSize = stream.ReadInt32()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FCompressedChunkInfo value)
    {
        stream.WriteInt32(value.CompressedSize);
        stream.WriteInt32(value.UncompressedSize);
    }
}