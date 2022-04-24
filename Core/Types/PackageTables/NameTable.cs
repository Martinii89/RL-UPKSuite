using Core.Serialization;
using Core.UnrealStream;
using Syroot.BinaryData;

namespace Core.Types.PackageTables;

public class NameTable
{
    public List<NameTableItem> Names { get; set; }

    public NameTable()
    {
        Names = new List<NameTableItem>();
    }

    public NameTable(Stream stream, long namesOffset, int namesCount)
    {
        stream.Position = namesOffset;
        Names = new List<NameTableItem>(namesCount);
        for (var index = 0; index < namesCount; index++)
        {
            var name = new NameTableItem();
            name.Deserialize(stream);
            Names.Add(name);
        }
    }

    public void Serialize(Stream outStream)
    {
        Names.ForEach(n => n.Serialize(outStream));
    }
}

public class NameTableItem 
{
    public string Name { get; set; } = string.Empty;
    public ulong Flags { get; set; }

    public void Deserialize(Stream reader)
    {
        Name = reader.ReadFString();
        Flags = reader.ReadUInt64();
    }

    public void Serialize(Stream outStream)
    {
        outStream.WriteFString(Name);
        outStream.WriteUInt64(Flags);
    }
}