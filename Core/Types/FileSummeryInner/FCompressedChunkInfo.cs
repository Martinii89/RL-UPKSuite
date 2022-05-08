namespace Core.Types.FileSummeryInner;

/// <summary>
///     Compressed data info
///     Rocket League stores this in the encrypted data rather than in the file summary!
/// </summary>
public class FCompressedChunkInfo
{
    private readonly int _licenseeVersion = 23;


    /// <summary>
    ///     Constructs a CompressedChunkInfo. The FileSummary is needed for the deserialize logic. Remove this once the new
    ///     serializers interfaces are fully implemented
    /// </summary>
    /// <param name="header"></param>
    public FCompressedChunkInfo(FileSummary header)
    {
        _licenseeVersion = header.LicenseeVersion;
    }

    /// <summary>
    ///     Default constructable to be able to use with TArray
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public FCompressedChunkInfo()
    {
    }

    /// <summary>
    ///     The offset into the data stream once uncompressed
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
}