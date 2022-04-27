using Core.Serialization;
using Core.Types.PackageTables;

namespace Core.Types;

/// <summary>
///     A UnrealPackage is the deserialized data from a UPK file. These files can contain all kinds of unreal object for a
///     game or specific maps..
/// </summary>
public class UnrealPackage : IBinaryDeserializableClass
{
    /// <summary>
    ///     The header summarises what the package contains and where in the file the different parts are located
    /// </summary>
    public FileSummary Header { get; } = new();

    /// <summary>
    ///     The name table contains all the names that this package references
    /// </summary>
    public NameTable NameTable { get; private set; } = new();

    /// <summary>
    ///     The import table references all the objects that this package depends on
    /// </summary>
    public ImportTable ImportTable { get; private set; } = new();

    /// <summary>
    ///     The export table contains all the objects that this package defines.
    /// </summary>
    public ExportTable ExportTable { get; private set; } = new();

    /// <summary>
    ///     The depends table will tell which objects each export depends on. For cooked packages this will be empty
    /// </summary>
    public DependsTable DependsTable { get; } = new();

    /// <summary>
    ///     The Thumbnail table contains data used by the editor to show thumbnails in the asset browser. Whenever a objetc has
    ///     a custom thumbnail, It will be defined here
    /// </summary>
    public ThumbnailTable ThumbnailTable { get; } = new();


    /// <summary>
    ///     Deserialized a package froma stream. Currently not fully implemented, it only deserializes the header and
    ///     name,import,export table. Objects are not constructed and imports are not linked with their package that exports
    ///     them.
    /// </summary>
    /// <param name="reader"></param>
    public void Deserialize(Stream reader)
    {
        Header.Deserialize(reader);

        NameTable = new NameTable(reader, Header.NameOffset, Header.NameCount);
        ImportTable = new ImportTable(reader, Header.ImportOffset, Header.ImportCount);
        ExportTable = new ExportTable(reader, Header.ExportOffset, Header.ExportCount);

        if (Header.CookerVersion == 0)
        {
            reader.Position = Header.DependsOffset;
            DependsTable.InitializeSize(Header.ExportCount);
            DependsTable.Deserialize(reader);

            reader.Position = Header.ThumbnailTableOffset;
            ThumbnailTable.Deserialize(reader);
        }
    }
}