using Core.Serialization;
using Core.Types.PackageTables;

namespace Core.Types;

public class UnrealPackage: IBinaryDeserializableClass
{
    public FileSummary Header { get; private set; } = new();
    public NameTable NameTable { get; private set; } = new();
    public ImportTable ImportTable { get; private set; } = new();
    public ExportTable ExportTable{ get; private set; } = new();
    public DependsTable DependsTable { get; private set; } = new();
    public ThumbnailTable ThumbnailTable  { get; private set; } = new();



    public void Deserialize(BinaryReader reader)
    {
        Header.Deserialize(reader);

        reader.BaseStream.Position = Header.NameOffset;
        NameTable = new NameTable(reader.BaseStream, Header.NameOffset, Header.NameCount);

        //reader.BaseStream.Position = Header.ImportOffset;
        //ImportTable.Deserialize(reader);

        //reader.BaseStream.Position = Header.ExportOffset;
        //ExportTable.Deserialize(reader);

        //reader.BaseStream.Position = Header.DependsOffset;
        //DependsTable.Deserialize(reader);

        //reader.BaseStream.Position = Header.ThumbnailTableOffset;
        //ThumbnailTable.Deserialize(reader);
    }
}