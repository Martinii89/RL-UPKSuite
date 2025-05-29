namespace RlUpk.Core.Types.FileSummeryInner;

/// <summary>
///     Flag used in <see cref="FileSummary.CompressionFlags" /> to indicate the compression of a package
/// </summary>
[Flags]
public enum ECompressionFlags
{
    /// <summary>
    ///     No compression
    /// </summary>
    CompressNone = 0x00,

    /// <summary>
    ///     Compressed with ZLIB
    /// </summary>
    CompressZlib = 0x01,

    /// <summary>
    ///     Compressed with GZIP
    /// </summary>
    CompressGzip = 0x02,

    /// <summary>
    ///     Prefer compression that compresses smaller (ONLY VALID FOR COMPRESSION)
    /// </summary>
    CompressBiasMemory = 0x10,

    /// <summary>
    ///     Prefer compression that compresses faster (ONLY VALID FOR COMPRESSION)
    /// </summary>
    CompressBiasSpeed = 0x20
}