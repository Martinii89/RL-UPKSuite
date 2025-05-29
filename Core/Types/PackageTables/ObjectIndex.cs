namespace RlUpk.Core.Types.PackageTables;

/// <summary>
///     References either a import or export object.
///     Values larger than zero are export objects. The real index in the export table will be  (index - 1 )
///     Values less than zero are import objects. The real index in the import table will be ( - index - 1 )
///     Zero is a null reference
/// </summary>
public readonly struct ObjectIndex : IEquatable<ObjectIndex>
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
        Index = 0;
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
    public int Index { get; }

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

    /// <inheritdoc />
    public bool Equals(ObjectIndex other)
    {
        return Index == other.Index;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ObjectIndex other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Index.GetHashCode();
    }

    public static bool operator ==(ObjectIndex left, ObjectIndex right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ObjectIndex left, ObjectIndex right)
    {
        return !(left == right);
    }
}