using Core.Types;

namespace Core.Compression;

/// <summary>
/// Flag used in <see cref="FileSummary.CompressionFlags"/> to indicate the compression of a package
/// </summary>
[Flags]
public enum ECompressionFlags : int
{
    /// <summary>
    /// No compression
    /// </summary>
    COMPRESS_None = 0x00,

    /// <summary>
    /// Compressed with ZLIB	
    /// </summary>
    COMPRESS_ZLIB = 0x01,

    /// <summary>
    /// Compressed with GZIP
    /// </summary>
    COMPRESS_GZIP = 0x02,

    /// <summary>
    /// Prefer compression that compresses smaller (ONLY VALID FOR COMPRESSION)
    /// </summary>
    COMPRESS_BiasMemory = 0x10,

    /// <summary>
    /// Prefer compression that compresses faster (ONLY VALID FOR COMPRESSION)
    /// </summary>
    COMPRESS_BiasSpeed = 0x20,
}