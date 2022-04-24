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

        NameTable = new NameTable(reader.BaseStream, Header.NameOffset, Header.NameCount);
        ImportTable = new ImportTable(reader.BaseStream, Header.ImportOffset, Header.ImportCount);
        ExportTable= new ExportTable(reader.BaseStream, Header.ExportOffset, Header.ExportCount);

        if (Header.CookerVersion == 0)
        {
            reader.BaseStream.Position = Header.DependsOffset;
            DependsTable.InitializeSize(Header.ExportCount);
            DependsTable.Deserialize(reader);

            reader.BaseStream.Position = Header.ThumbnailTableOffset;
            ThumbnailTable.Deserialize(reader.BaseStream);
        }



    }
}