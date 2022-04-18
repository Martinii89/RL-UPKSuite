using Core.Serialization;

namespace Core.Types.FileSummeryInner;

public class FTextureType : IBinaryDeserializableClass
{
    public int SizeX { get; private set; }
    public int SizeY { get; private set; }
    public int NumMips { get; private set; }
    public int Format { get; private set; }
    public int TexCreateFlags { get; private set; }
    public TArray<int> ExportIndices { get; } = new();
    public void Deserialize(BinaryReader reader)
    {
        SizeX = reader.ReadInt32();
        SizeY = reader.ReadInt32(); ;
        NumMips = reader.ReadInt32(); ;
        Format = reader.ReadInt32(); ;
        TexCreateFlags = reader.ReadInt32(); ;
        ExportIndices.Deserialize(reader);
    }
}