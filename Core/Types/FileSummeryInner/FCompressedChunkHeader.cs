namespace RlUpk.Core.Types.FileSummeryInner;

internal class FCompressedChunkHeader
{
    public int Tag { get; init; }
    public int BlockSize { get; init; }
    public FCompressedChunkInfo Summary { get; init; } = new(); // Total of all blocks

    public int BlockCount => (Summary.UncompressedSize + BlockSize + 1) / BlockSize;
}