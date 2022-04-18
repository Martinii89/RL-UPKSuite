using Core.Serialization;

namespace Core.Types.FileSummeryInner;

public class FGenerationInfo : IBinaryDeserializableClass
{
    public int ExportCount { get; private set; }
    public int NameCount { get; private set; }

    public int NetObjectCount { get; private set; }

    public void Deserialize(BinaryReader reader)
    {
        ExportCount = reader.ReadInt32();
        NameCount = reader.ReadInt32();
        NetObjectCount = reader.ReadInt32();
    }
}