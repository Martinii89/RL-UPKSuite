namespace Core.Types.FileSummeryInner;

internal class FCompressedChunkHeader
{
    public int Tag { get; init; }
    public int BlockSize { get; init; }
    public FCompressedChunkBlock Summary { get; init; } = new(); // Total of all blocks

    public int BlockCount => (Summary.UncompressedSize + BlockSize + 1) / BlockSize;
}

internal class FCompressedChunkBlock
{
    public int CompressedSize { get; init; }
    public int UncompressedSize { get; init; }
}