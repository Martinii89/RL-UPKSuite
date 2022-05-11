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

    /// <summary>
    ///     Uses a given serializer to deserialize the names in the stream
    /// </summary>
    /// <param name="serializer"></param>
    /// <param name="stream"></param>
    /// <param name="nameCount"></param>
    public NameTable(IStreamSerializerFor<NameTableItem> serializer, Stream stream, int nameCount)
    {
        serializer.ReadTArrayToList(stream, this, nameCount);
        //AddRange(serializer.ReadTArray(stream, nameCount));
    }
}

/// <summary>
///     A NameTableItem contains a name as a string and some related flag data. I don't know what the flags are used for
/// </summary>
public readonly struct NameTableItem
{
    /// <summary>
    ///     The name as a string
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     A bit-flag of unknown significance
    /// </summary>
    public ulong Flags { get; }


    /// <summary>
    ///     Construct a public name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="flags"></param>
    public NameTableItem(string name, ulong flags)
    {
        Name = name;
        Flags = flags;
    }
}