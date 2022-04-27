namespace Core.Types.PackageTables;

/// <summary>
///     The Export table contains a <see cref="ExportTableItem" /> for every exported object in a package
/// </summary>
public class ExportTable
{
    /// <summary>
    ///     Initialize a empty table
    /// </summary>
    public ExportTable()
    {
        Exports = new List<ExportTableItem>();
    }

    /// <summary>
    ///     Initialize a table with given export items
    /// </summary>
    /// <param name="exports"></param>
    public ExportTable(List<ExportTableItem> exports)
    {
        Exports = exports;
    }

    /// <summary>
    ///     Initialize and deserialize the table from a stream
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <param name="exportOffset">The offset in the stream where the table data starts</param>
    /// <param name="exportCount">The number of exports to deserialize</param>
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

    /// <summary>
    ///     The list of export items
    /// </summary>
    public List<ExportTableItem> Exports { get; set; }

    /// <summary>
    ///     Write the table data to the stream. Does not write the amount of exports, only the data for each
    ///     <see cref="ExportTableItem" />
    /// </summary>
    /// <param name="outStream"></param>
    public void Serialize(Stream outStream)
    {
        Exports.ForEach(n => n.Serialize(outStream));
    }
}

/// <summary>
///     A ExportTableItem represents the meta data of a export object in a package. Things such as class, object name, base
///     type, outer object and serialization size\offset is stored here. It does not track it's own index in the export
///     table.
/// </summary>
public class ExportTableItem
{
    /// <summary>
    ///     All fields have their default values. Used to construct a default object that you can later call deserialize on
    ///     with a stream to get real data.
    /// </summary>
    public ExportTableItem()
    {
    }

    /// <summary>
    ///     Injects all the members in the constructor. Used for unit tests and construction injection.
    /// </summary>
    /// <param name="classIndex"></param>
    /// <param name="superIndex"></param>
    /// <param name="outerIndex"></param>
    /// <param name="objectName"></param>
    /// <param name="archetypeIndex"></param>
    /// <param name="objectFlags"></param>
    /// <param name="serialSize"></param>
    /// <param name="serialOffset"></param>
    /// <param name="exportFlags"></param>
    /// <param name="netObjects"></param>
    /// <param name="packageGuid"></param>
    /// <param name="packageFlags"></param>
    public ExportTableItem(ObjectIndex classIndex, ObjectIndex superIndex, ObjectIndex outerIndex, FName objectName,
        ObjectIndex archetypeIndex,
        ulong objectFlags, int serialSize, long serialOffset, int exportFlags, TArray<int> netObjects,
        FGuid packageGuid, int packageFlags)
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

    /// <summary>
    ///     A reference to the UClass for this object. A value of zero indicates this is a UClass definition.
    /// </summary>
    public ObjectIndex ClassIndex { get; } = new();

    /// <summary>
    ///     A reference to the base type of this object. If null this object has no base.
    /// </summary>
    public ObjectIndex SuperIndex { get; } = new();

    /// <summary>
    ///     A reference to the outer object. If null this is a top level package.
    /// </summary>
    public ObjectIndex OuterIndex { get; } = new();

    /// <summary>
    ///     The name of this object.
    /// </summary>
    public FName ObjectName { get; } = new();

    /// <summary>
    ///     A reference to the archetype of this object. If null this object is not a archetype
    /// </summary>
    public ObjectIndex ArchetypeIndex { get; } = new();

    /// <summary>
    ///     ObjectFlags. Unsure what these are used for
    /// </summary>
    public ulong ObjectFlags { get; private set; }

    /// <summary>
    ///     How many bytes are serialized to disk for this object
    /// </summary>
    public int SerialSize { get; private set; }

    /// <summary>
    ///     Where the serialized data is located
    /// </summary>
    public long SerialOffset { get; private set; }

    /// <summary>
    ///     Export specific flags
    /// </summary>
    public int ExportFlags { get; private set; }

    /// <summary>
    ///     The number of net serializable objects contained within this object
    /// </summary>
    public TArray<int> NetObjects { get; } = new();

    /// <summary>
    ///     The GUID of the original package
    /// </summary>
    public FGuid PackageGuid { get; } = new();

    /// <summary>
    ///     Flags of the original package.
    /// </summary>
    public int PackageFlags { get; private set; }

    /// <summary>
    ///     Deserialze the members from the stream
    /// </summary>
    /// <param name="stream"></param>
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
        NetObjects.Deserialize(stream);
        PackageGuid.Deserialize(stream);
        PackageFlags = stream.ReadInt32();
    }

    /// <summary>
    ///     Serialize the members to the stream
    /// </summary>
    /// <param name="stream"></param>
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