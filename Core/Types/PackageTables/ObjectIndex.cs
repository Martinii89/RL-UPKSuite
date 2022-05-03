namespace Core.Types.PackageTables;

/// <summary>
///     References either a import or export object.
///     Values larger than zero are export objects. The real index in the export table will be  (index - 1 )
///     Values less than zero are import objects. The real index in the import table will be ( - index - 1 )
///     Zero is a null reference
/// </summary>
public class ObjectIndex
{
    /// <summary>
    ///     From which table can you find the reference object
    /// </summary>
    public enum ReferencedTable
    {
        /// <summary>
        ///     This is a null reference
        /// </summary>
        Null,

        /// <summary>
        ///     This is a import object reference
        /// </summary>
        Import,

        /// <summary>
        ///     This is a export object reference
        /// </summary>
        Export
    }

    /// <summary>
    ///     Constructs a null reference
    /// </summary>
    public ObjectIndex()
    {
    }

    /// <summary>
    ///     Constructs a reference with a given index
    /// </summary>
    /// <param name="index"></param>
    public ObjectIndex(int index)
    {
        Index = index;
    }

    /// <summary>
    ///     The reference index. Larger than zero is a export. Less than zero is a import. Zero is a null reference
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    ///     Returns the index for the reference in the export table
    /// </summary>
    /// <returns></returns>
    public int ExportIndex => Index - 1;

    /// <summary>
    ///     Returns the index for the reference in the import table
    /// </summary>
    /// <returns></returns>
    public int ImportIndex => -Index - 1;

    /// <summary>
    ///     Converts a export table index to a object reference index
    /// </summary>
    /// <param name="exportIndex"></param>
    /// <returns></returns>
    public static int FromExportIndex(int exportIndex)
    {
        return exportIndex + 1;
    }

    /// <summary>
    ///     Converts a import table index to a object reference index
    /// </summary>
    /// <param name="importIndex"></param>
    /// <returns></returns>
    public static int FromImportIndex(int importIndex)
    {
        return -importIndex - 1;
    }

    /// <summary>
    ///     Which table does this object reference
    /// </summary>
    /// <returns></returns>
    public ReferencedTable GetReferencedTable()
    {
        return Index switch
        {
            0 => ReferencedTable.Null,
            < 0 => ReferencedTable.Import,
            > 0 => ReferencedTable.Export
        };
    }


    /// <summary>
    ///     Reads a int32 value from the stream and assigns this as the Index
    /// </summary>
    /// <param name="stream"></param>
    public void Deserialize(Stream stream)
    {
        Index = stream.ReadInt32();
    }

    /// <summary>
    ///     Writes the index to the stream as a in32 value
    /// </summary>
    /// <param name="stream"></param>
    public void Serialize(Stream stream)
    {
        stream.WriteInt32(Index);
    }
}