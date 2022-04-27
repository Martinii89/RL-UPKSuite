using Core.Serialization;

namespace Core.Types.FileSummeryInner;

// Compressed data info
// Rocket League stores this in the encrypted data rather than in the file summary
public class FCompressedChunkInfo : IBinaryDeserializableClass
{
    private readonly FileSummary _header;


    public FCompressedChunkInfo(FileSummary header)
    {
        _header = header;
    }

    public FCompressedChunkInfo()
    {
        throw new NotImplementedException();
    }

    public long UncompressedOffset { get; internal set; }
    public long CompressedOffset { get; internal set; }
    public int UncompressedSize { get; internal set; }
    public int CompressedSize { get; internal set; }

    public void Deserialize(Stream reader)
    {
        UncompressedOffset = _header.LicenseeVersion >= 22 ? reader.ReadInt64() : reader.ReadInt32();
        UncompressedSize = reader.ReadInt32();
        CompressedOffset = _header.LicenseeVersion >= 22 ? reader.ReadInt64() : reader.ReadInt32();
        CompressedSize = reader.ReadInt32();
    }
}