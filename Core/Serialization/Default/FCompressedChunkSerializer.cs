using Core.Types.FileSummeryInner;

namespace Core.Serialization.Default;

/// <inheritdoc />
public class FCompressedChunkSerializer : IStreamSerializer<FCompressedChunk>
{
    /// <inheritdoc />
    public FCompressedChunk Deserialize(Stream stream)
    {
        return new FCompressedChunk
        {
            UncompressedOffset = stream.ReadInt64(),
            UncompressedSize = stream.ReadInt32(),
            CompressedOffset = stream.ReadInt64(),
            CompressedSize = stream.ReadInt32()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FCompressedChunk value)
    {
        stream.WriteInt32((int) value.UncompressedOffset);
        stream.WriteInt32(value.UncompressedSize);
        stream.WriteInt32((int) value.CompressedOffset);
        stream.WriteInt32(value.CompressedSize);
    }
}