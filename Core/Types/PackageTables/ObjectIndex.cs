using Syroot.BinaryData;

namespace Core.Types.PackageTables;

public class ObjectIndex
{
    public ObjectIndex()
    {
    }

    public ObjectIndex(int index)
    {
        Index = index;
    }

    public int Index { get; private set; }

    public void Deserialize(Stream stream)
    {
        Index = stream.ReadInt32();
    }

    public void Serialize(Stream stream)
    {
        stream.WriteInt32(Index);
    }
}