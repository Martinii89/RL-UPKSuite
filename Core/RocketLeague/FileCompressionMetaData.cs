namespace Core.RocketLeague;

/// <summary>
///     Compression data about where to find the compressed chunks in the compressed rocket league package
/// </summary>
internal class FileCompressionMetaData
{
    // Number of bytes of (pos % 0xFF) at the end of the decrypted data, I don't know why it's needed
    internal int GarbageSize { get; set; }

    // Offset to TArray<FCompressedChunk> in decrypted data
    internal int CompressedChunksffset { get; set; }

    // Size of the last AES block in the encrypted data
    internal int LastBlockSize { get; set; }


    /// <summary>
    ///     Deserialize the fields. Will only give valid results when the stream is placed at the end of the conventional
    ///     fileSummary.
    /// </summary>
    /// <param name="reader"></param>
    internal static FileCompressionMetaData Deserialize(Stream reader)
    {
        return new FileCompressionMetaData
        {
            GarbageSize = reader.ReadInt32(),
            CompressedChunksffset = reader.ReadInt32(),
            LastBlockSize = reader.ReadInt32()
        };
    }
}