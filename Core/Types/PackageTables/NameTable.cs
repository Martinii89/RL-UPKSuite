using Core.Extensions;

namespace Core.Types.PackageTables;

/// <summary>
///     A name table contains all the names for a package. The index for <see cref="FName" /> objects are referencing the
///     index of this table
/// </summary>
public class NameTable
{
    /// <summary>
    ///     Construct a empty name table
    /// </summary>
    public NameTable()
    {
        Names = new List<NameTableItem>();
    }

    /// <summary>
    ///     Initialize and deserialize the table from a stream
    /// </summary>
    /// <param name="stream">The input stream</param>
    /// <param name="namesOffset">The offset in the stream where the name table starts </param>
    /// <param name="namesCount">The numbers of names to deserialize</param>
    public NameTable(Stream stream, long namesOffset, int namesCount)
    {
        stream.Position = namesOffset;
        Names = new List<NameTableItem>(namesCount);
        for (var index = 0; index < namesCount; index++)
        {
            var name = new NameTableItem();
            name.Deserialize(stream);
            Names.Add(name);
        }
    }

    /// <summary>
    ///     The list of names. <see cref="FName" /> indexes are indexing into the list
    /// </summary>
    public List<NameTableItem> Names { get; set; }

    /// <summary>
    ///     Write the table data to the stream. Does not write the amount of exports, only the data for each
    ///     <see cref="NameTableItem" />
    /// </summary>
    /// <param name="outStream"></param>
    public void Serialize(Stream outStream)
    {
        Names.ForEach(n => n.Serialize(outStream));
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

    /// <summary>
    ///     Deserialize the name (as a FString) and the flag value from the stream
    /// </summary>
    /// <param name="reader"></param>
    public void Deserialize(Stream reader)
    {
        Name = reader.ReadFString();
        Flags = reader.ReadUInt64();
    }

    /// <summary>
    ///     Write the name (as a FString) and the flag to the stream
    /// </summary>
    /// <param name="outStream"></param>
    public void Serialize(Stream outStream)
    {
        outStream.WriteFString(Name);
        outStream.WriteUInt64(Flags);
    }
}