namespace Core.RocketLeague;

/// <summary>
///     Compression data about where to find the compresed chunks in the compresed rocket league package
/// </summary>
internal class FileCompressionMetaData
{
    // Number of bytes of (pos % 0xFF) at the end of the decrypted data, I don't know why it's needed
    internal int GarbageSize { get; set; }

    // Offset to TArray<FCompressedChunkInfo> in decrypted data
    internal int CompressedChunkInfoOffset { get; set; }

    // Size of the last AES block in the encrypted data
    internal int LastBlockSize { get; set; }


    /// <summary>
    ///     Deserialize the fields. Will only give valid results when the stream is placed at the end of the conventional
    ///     filesummary.
    /// </summary>
    /// <param name="reader"></param>
    internal void Deserialize(Stream reader)
    {
        GarbageSize = reader.ReadInt32();
        CompressedChunkInfoOffset = reader.ReadInt32();
        LastBlockSize = reader.ReadInt32();
    }
}