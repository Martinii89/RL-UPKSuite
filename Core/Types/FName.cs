using Core.Types.PackageTables;
using Syroot.BinaryData;

namespace Core.Types;

public class FName
{
    public int NameIndex { get; private set; }
    public int Number { get; private set; }

    public FName()
    {
    }

    public FName(int nameIndex, int number = 0)
    {
        NameIndex = nameIndex;
        Number = number;
    }

    public FName(Stream reader)
    {
        Deserialize(reader);
    }

    public void Deserialize(Stream reader)
    {
        NameIndex = reader.ReadInt32();
        Number = reader.ReadInt32();
    }

    public void Serialize(Stream writer)
    {
        writer.WriteInt32(NameIndex);
        writer.WriteInt32(Number);
    }
}