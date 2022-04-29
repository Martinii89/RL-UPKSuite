using Core.Serialization;

namespace Core.Types.FileSummeryInner;

/// <summary>
///     Compressed data info
///     Rocket League stores this in the encrypted data rather than in the file summary|
/// </summary>
public class FCompressedChunkInfo : IBinaryDeserializableClass
{
    private readonly FileSummary _header;


    /// <summary>
    ///     Constructs a CompressedChunkInfo. The FileSummary is needed for the deserialize logic. Remove this once the new
    ///     serializers interfaces are fully implemented
    /// </summary>
    /// <param name="header"></param>
    public FCompressedChunkInfo(FileSummary header)
    {
        _header = header;
    }

    /// <summary>
    ///     Default constructible to be able to use with TArray
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public FCompressedChunkInfo()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     The offset into the datastream ince uncompressed
    /// </summary>
    public long UncompressedOffset { get; internal set; }

    /// <summary>
    ///     The offset where the compressed data starts
    /// </summary>
    public long CompressedOffset { get; internal set; }

    /// <summary>
    ///     The size once uncompressed
    /// </summary>
    public int UncompressedSize { get; internal set; }

    /// <summary>
    ///     The size of the compressed data
    /// </summary>
    public int CompressedSize { get; internal set; }

    /// <summary>
    ///     Deserialize the data. Uses the header to decide between old and new format specification.
    /// </summary>
    /// <param name="reader"></param>
    public void Deserialize(Stream reader)
    {
        UncompressedOffset = _header.LicenseeVersion >= 22 ? reader.ReadInt64() : reader.ReadInt32();
        UncompressedSize = reader.ReadInt32();
        CompressedOffset = _header.LicenseeVersion >= 22 ? reader.ReadInt64() : reader.ReadInt32();
        CompressedSize = reader.ReadInt32();
    }
}