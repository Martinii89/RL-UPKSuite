using Core.Classes.Core;
using Core.Serialization;

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
    ///     Initialize and deserialize the table from the stream. Requires a serializer, the offset where the table data
    ///     starts, and how many
    ///     items to deserialize
    /// </summary>
    /// <param name="serializer"></param>
    /// <param name="stream"></param>
    /// <param name="importCount"></param>
    public ImportTable(IStreamSerializerFor<ImportTableItem> serializer, Stream stream, int importCount)
    {
        serializer.ReadTArrayToList(stream, this, importCount);
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


    /// <summary>
    ///     The imported object. Null until after the later stages of package loading.
    /// </summary>
    public UObject? ImportedObject { get; set; }


    /// <inheritdoc />
    public ObjectIndex OuterIndex { get; set; } = new();


    /// <inheritdoc />
    public FName ObjectName { get; set; } = new();
}