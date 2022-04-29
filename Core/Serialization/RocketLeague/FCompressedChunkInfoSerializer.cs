using Core.Types.FileSummeryInner;

namespace Core.Serialization.RocketLeague;

/// <inheritdoc />
public class FCompressedChunkInfoSerializer : RocketLeagueBase, IStreamSerializerFor<FCompressedChunkInfo>
{
    /// <inheritdoc />
    public FCompressedChunkInfo Deserialize(Stream stream)
    {
        return new FCompressedChunkInfo
        {
            UncompressedOffset = stream.ReadInt64(),
            UncompressedSize = stream.ReadInt32(),
            CompressedOffset = stream.ReadInt64(),
            CompressedSize = stream.ReadInt32()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FCompressedChunkInfo value)
    {
        throw new NotImplementedException();
    }
}