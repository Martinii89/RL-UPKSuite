using Core.Classes.Core;

namespace Core.Types.PackageTables;

/// <summary>
///     The ImportTable contains a <see cref="ImportTableItem" /> for every imported item in a package.
///     Imports are objects that the exported objects depend on.
///     These could be among other things: Classes, structs, textures, meshes.
///     Pretty much anything required to fully construct a instance of a export.
/// </summary>
public class ImportTable : List<ImportTableItem>
{
    /// <summary>
    ///     Initialized the table with a empty list
    /// </summary>
    public ImportTable()
    {
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
        Capacity = importsCount;
        for (var index = 0; index < importsCount; index++)
        {
            var name = new ImportTableItem();
            name.Deserialize(stream);
            Add(name);
        }
    }

    internal UnrealPackage UnrealPackage { get; set; }


    /// <summary>
    ///     Serialize the imports to the stream. Does not write the amount of imports to the stream, only the table items are
    ///     written.
    /// </summary>
    /// <param name="outStream"></param>
    public void Serialize(Stream outStream)
    {
        ForEach(n => n.Serialize(outStream));
    }
}

/// <summary>
///     A ImportTableItem contains the metadata about a import object. It's name, type, outer, and which package it is
///     exported from. It does not track it's own index in the import table.
/// </summary>
public class ImportTableItem : IObjectResource
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
    /// <param name="outerIndex"></param>
    /// <param name="objectName"></param>
    public ImportTableItem(FName classPackage, FName className, ObjectIndex outerIndex, FName objectName)
    {
        ClassPackage = classPackage;
        ClassName = className;
        OuterIndex = outerIndex;
        ObjectName = objectName;
    }

    /// <summary>
    ///     The name of the package this object can be found in
    /// </summary>
    public FName ClassPackage { get; set; } = new();

    /// <summary>
    ///     The name of the class
    /// </summary>
    public FName ClassName { get; set; } = new();


    public UObject? ImportedObject { get; set; }


    /// <inheritdoc />
    public ObjectIndex OuterIndex { get; set; } = new();


    /// <inheritdoc />
    public FName ObjectName { get; set; } = new();

    /// <summary>
    ///     Deserialize the import metadata from the stream
    /// </summary>
    /// <param name="stream"></param>
    public void Deserialize(Stream stream)
    {
        ClassPackage.Deserialize(stream);
        ClassName.Deserialize(stream);
        OuterIndex.Deserialize(stream);
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
        OuterIndex.Serialize(stream);
        ObjectName.Serialize(stream);
    }
}