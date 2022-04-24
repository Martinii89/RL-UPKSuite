using Core.Serialization;
using Syroot.BinaryData;

namespace Core.Types.PackageTables;

public class ExportTable
{
    public List<ExportTableItem> Exports { get; set; }

    public ExportTable()
    {
        Exports = new List<ExportTableItem>();
    }

    public ExportTable(List<ExportTableItem> exports)
    {
        Exports = exports;
    }

    public ExportTable(Stream stream, int exportOffset, int exportCount)
    {
        stream.Position = exportOffset;
        Exports = new List<ExportTableItem>(exportCount);
        for (var index = 0; index < exportCount; index++)
        {
            var name = new ExportTableItem();
            name.Deserialize(stream);
            Exports.Add(name);
        }
    }

    public void Serialize(Stream outStream)
    {
        Exports.ForEach(n => n.Serialize(outStream));
    }
}

public class ExportTableItem
{
    public ObjectIndex ClassIndex { get; private set; } = new();
    public ObjectIndex SuperIndex { get; private set; } = new();
    public ObjectIndex OuterIndex { get; private set; } = new();
    public FName ObjectName { get; private set; } = new();
    public ObjectIndex ArchetypeIndex { get; private set; } = new();
    public ulong ObjectFlags { get; private set; }
    public int SerialSize { get; private set; }
    public long SerialOffset { get; private set; }
    public int ExportFlags { get; private set; }
    public TArray<int> NetObjects { get; private set; } = new();
    public FGuid PackageGuid { get; private set; } = new();
    public int PackageFlags { get; private set; }

    public ExportTableItem()
    {
    }

    public ExportTableItem(ObjectIndex classIndex, ObjectIndex superIndex, ObjectIndex outerIndex, FName objectName, ObjectIndex archetypeIndex,
        ulong objectFlags, int serialSize, long serialOffset, int exportFlags, TArray<int> netObjects, FGuid packageGuid, int packageFlags)
    {
        ClassIndex = classIndex;
        SuperIndex = superIndex;
        OuterIndex = outerIndex;
        ObjectName = objectName;
        ArchetypeIndex = archetypeIndex;
        ObjectFlags = objectFlags;
        SerialSize = serialSize;
        SerialOffset = serialOffset;
        ExportFlags = exportFlags;
        NetObjects = netObjects;
        PackageGuid = packageGuid;
        PackageFlags = packageFlags;
    }

    public void Deserialize(Stream stream)
    {
        ClassIndex.Deserialize(stream);
        SuperIndex.Deserialize(stream);
        OuterIndex.Deserialize(stream);
        ObjectName.Deserialize(stream);
        ArchetypeIndex.Deserialize(stream);
        ObjectFlags = stream.ReadUInt64();
        SerialSize = stream.ReadInt32();
        SerialOffset = stream.ReadInt64();
        ExportFlags = stream.ReadInt32();
        // TODO fix that interface
        var binaryReader = new BinaryReader(stream);
        NetObjects.Deserialize(binaryReader);
        PackageGuid.Deserialize(binaryReader);
        PackageFlags = stream.ReadInt32();
    }

    public void Serialize(Stream stream)
    {
        ClassIndex.Serialize(stream);
        SuperIndex.Serialize(stream);
        OuterIndex.Serialize(stream);
        ObjectName.Serialize(stream);
        ArchetypeIndex.Serialize(stream);
        stream.WriteUInt64(ObjectFlags);
        stream.WriteInt32(SerialSize);
        stream.WriteInt64(SerialOffset);
        stream.WriteInt32(ExportFlags);
        NetObjects.Serialize(stream);
        PackageGuid.Serialize(stream);
        stream.WriteInt32(PackageFlags);
    }
}