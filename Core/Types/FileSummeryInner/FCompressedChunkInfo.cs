using Core.Serialization;

namespace Core.Types.FileSummeryInner;

// Compressed data info
// Rocket League stores this in the encrypted data rather than in the file summary
internal class FCompressedChunkInfo : IBinaryDeserializableClass
{
    public long UncompressedOffset { get; private set; }
    public long CompressedOffset { get; private set; }
    public int UncompressedSize { get; private set; }
    public int CompressedSize { get; private set; }

    private readonly FileSummary _header;


    public FCompressedChunkInfo(FileSummary header)
    {
        _header = header;
    }

    public FCompressedChunkInfo()
    {
        throw new NotImplementedException();
    }

    public void Deserialize(BinaryReader reader)
    {
        UncompressedOffset = _header.LicenseeVersion >= 22 ? reader.ReadInt64() : reader.ReadInt32();
        UncompressedSize = reader.ReadInt32();
        CompressedOffset = _header.LicenseeVersion >= 22 ? reader.ReadInt64() : reader.ReadInt32();
        CompressedSize = reader.ReadInt32();
    }
}