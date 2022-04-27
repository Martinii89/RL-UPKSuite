using Core.Types;
using Core.Types.FileSummeryInner;

namespace Core.Serialization.RocketLeage;

public class FCompressedChunkInfoSerializer : RocketLeagueBase, IStreamSerializerFor<TArray<FCompressedChunkInfo>>
{
    public TArray<FCompressedChunkInfo> Deserialize(Stream stream)
    {
        var arraySize = stream.ReadInt32();
        var chunkInfos = new TArray<FCompressedChunkInfo>
        {
            Capacity = arraySize
        };
        for (var i = 0; i < arraySize; i++)
        {
            chunkInfos.Add(new FCompressedChunkInfo
            {
                UncompressedOffset = stream.ReadInt64(),
                UncompressedSize = stream.ReadInt32(),
                CompressedOffset = stream.ReadInt64(),
                CompressedSize = stream.ReadInt32()
            });
        }

        return chunkInfos;
    }

    public void Serialize(Stream stream, TArray<FCompressedChunkInfo> value)
    {
        throw new NotImplementedException();
    }
}