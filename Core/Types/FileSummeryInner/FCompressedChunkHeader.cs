using Core.Serialization;

namespace Core.Types.FileSummeryInner;

internal class FCompressedChunkHeader : IBinaryDeserializableClass
{
    public int Tag { get; private set; }
    public int BlockSize { get; private set; }
    public FCompressedChunkBlock Summary { get; } = new(); // Total of all blocks

    public int BlockCount => (Summary.UncompressedSize + BlockSize + 1) / BlockSize;

    public void Deserialize(Stream reader)
    {
        Tag = reader.ReadInt32();
        BlockSize = reader.ReadInt32();
        Summary.Deserialize(reader);
    }
}

internal class FCompressedChunkBlock : IBinaryDeserializableClass
{
    public int CompressedSize { get; private set; }
    public int UncompressedSize { get; private set; }

    public void Deserialize(Stream reader)
    {
        CompressedSize = reader.ReadInt32();
        UncompressedSize = reader.ReadInt32();
    }
}