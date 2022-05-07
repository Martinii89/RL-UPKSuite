using Core.Serialization;

namespace Core.Types.PackageTables;

/// <summary>
///     A name table contains all the names for a package. The index for <see cref="FName" /> objects are referencing the
///     index of this table
/// </summary>
public class NameTable : List<NameTableItem>
{
    /// <summary>
    ///     Construct a empty name table
    /// </summary>
    public NameTable()
    {
    }

    public NameTable(IStreamSerializerFor<NameTableItem> serializer, Stream stream, int nameCount)
    {
        AddRange(serializer.ReadTArray(stream, nameCount));
    }
}

/// <summary>
///     A NameTableItem contains a name as a string and some related flag data. I don't know what the flags are used for
/// </summary>
public class NameTableItem
{
    /// <summary>
    ///     The name as a string
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     A bit-flag of unknown significance
    /// </summary>
    public ulong Flags { get; set; }
}