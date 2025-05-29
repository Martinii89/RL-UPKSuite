namespace RlUpk.Core.Types.FileSummeryInner;

/// <summary>
///     Compressed data info
///     Rocket League stores this in the encrypted data rather than in the file summary!
/// </summary>
public class FCompressedChunk
{
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