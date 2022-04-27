namespace Core.Types.PackageTables;

/// <summary>
///     The ImportTable contains a <see cref="ImportTableItem" /> for every imported item in a package.
///     Imports are objects that the exported objects depend on.
///     These could be among other things: Classes, structs, textures, meshes.
///     Pretty much anything required to fully construct a instance of a export.
/// </summary>
public class ImportTable
{
    /// <summary>
    ///     Initialized the table with a empty list
    /// </summary>
    public ImportTable()
    {
        Imports = new List<ImportTableItem>();
    }

    /// <summary>
    ///     Initialize the table with a given list
    /// </summary>
    /// <param name="imports"></param>
    public ImportTable(List<ImportTableItem> imports)
    {
        Imports = imports;
    }

    /// <summary>
    ///     Initialize and deserialize the table from the stream. Requires the offset where the table data starts, and how many
    ///     items to deserialize
    /// </summary>
    /// <param name="stream">The intput stream</param>
    /// <param name="importsOffset">The offset into the stream where the serial data starts</param>
    /// <param name="importsCount">How many import items to deserialize</param>
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

    /// <summary>
    ///     The list of import items
    /// </summary>
    public List<ImportTableItem> Imports { get; set; }

    /// <summary>
    ///     Serialize the imports to the stream. Does not write the amount of imports to the stream, only the table items are
    ///     written.
    /// </summary>
    /// <param name="outStream"></param>
    public void Serialize(Stream outStream)
    {
        Imports.ForEach(n => n.Serialize(outStream));
    }
}

/// <summary>
///     A ImportTableItem contains the metadata about a import object. It's name, type, outer, and which package it is
///     exported from. It does not track it's own index in the import table.
/// </summary>
public class ImportTableItem
{
    /// <summary>
    ///     A empty import item. It's main use is to have something to populate with Deserialize
    /// </summary>
    public ImportTableItem()
    {
    }

    /// <summary>
    ///     Construct a fully defined import item.
    /// </summary>
    /// <param name="classPackage"></param>
    /// <param name="className"></param>
    /// <param name="outer"></param>
    /// <param name="objectName"></param>
    public ImportTableItem(FName classPackage, FName className, ObjectIndex outer, FName objectName)
    {
        ClassPackage = classPackage;
        ClassName = className;
        Outer = outer;
        ObjectName = objectName;
    }

    /// <summary>
    ///     The name of the package this object can be found in
    /// </summary>
    public FName ClassPackage { get; } = new();

    /// <summary>
    ///     The name of the class
    /// </summary>
    public FName ClassName { get; } = new();

    /// <summary>
    ///     A reference to the outer object of this import
    /// </summary>
    public ObjectIndex Outer { get; } = new();

    /// <summary>
    ///     The name of the import object
    /// </summary>
    public FName ObjectName { get; } = new();


    /// <summary>
    ///     Deserialize the import metadata from the stream
    /// </summary>
    /// <param name="stream"></param>
    public void Deserialize(Stream stream)
    {
        ClassPackage.Deserialize(stream);
        ClassName.Deserialize(stream);
        Outer.Deserialize(stream);
        ObjectName.Deserialize(stream);
    }

    /// <summary>
    ///     Serialize the object metadata to the stream.
    /// </summary>
    /// <param name="stream"></param>
    public void Serialize(Stream stream)
    {
        ClassPackage.Serialize(stream);
        ClassName.Serialize(stream);
        Outer.Serialize(stream);
        ObjectName.Serialize(stream);
    }
}