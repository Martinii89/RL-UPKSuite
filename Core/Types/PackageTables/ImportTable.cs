using Core.Serialization;

namespace Core.Types.PackageTables;

public class ImportTable
{
    public List<ImportTableItem> Imports { get; set; }

    public ImportTable()
    {
        Imports = new List<ImportTableItem>();
    }

    public ImportTable(List<ImportTableItem> imports)
    {
        Imports = imports;
    }

    public ImportTable(Stream stream, long importsOffset, int importsCount)
    {
        stream.Position = importsOffset;
        Imports = new List<ImportTableItem>(importsCount);
        for (var index = 0; index < importsCount; index++)
        {
            var name = new ImportTableItem();
            name.Deserialize(stream);
            Imports.Add(name);
        }
    }

    public void Serialize(Stream outStream)
    {
        Imports.ForEach(n => n.Serialize(outStream));
    }
}

public class ImportTableItem
{
    public FName ClassPackage { get; private set; } = new();
    public FName ClassName { get; private set; } = new();
    public ObjectIndex Outer { get; private set; } = new();
    public FName ObjectName { get; private set; } = new();

    public ImportTableItem()
    {
    }

    public ImportTableItem(FName classPackage, FName className, ObjectIndex outer, FName objectName)
    {
        ClassPackage = classPackage;
        ClassName = className;
        Outer = outer;
        ObjectName = objectName;
    }


    public void Deserialize(Stream stream)
    {
        ClassPackage.Deserialize(stream);
        ClassName.Deserialize(stream);
        Outer.Deserialize(stream);
        ObjectName.Deserialize(stream);
    }

    public void Serialize(Stream stream)
    {
        ClassPackage.Serialize(stream);
        ClassName.Serialize(stream);
        Outer.Serialize(stream);
        ObjectName.Serialize(stream);
    }
}