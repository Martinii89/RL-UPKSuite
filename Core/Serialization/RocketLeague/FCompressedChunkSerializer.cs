using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types.FileSummeryInner;

namespace RlUpk.Core.Serialization.RocketLeague;

/// <inheritdoc />
[FileVersion(RocketLeagueBase.FileVersion)]
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
        stream.WriteInt64(value.UncompressedOffset);
        stream.WriteInt32(value.UncompressedSize);
        stream.WriteInt64(value.CompressedOffset);
        stream.WriteInt32(value.CompressedSize);
    }
}