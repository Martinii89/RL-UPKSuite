using System.Diagnostics;

using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;

namespace RlUpk.Core.Types.PackageTables;

/// <summary>
///     A name table contains all the names for a package. The index for <see cref="FName" /> objects are referencing the
///     index of this table
/// </summary>
public class NameTable : List<NameTableItem>
{

    private Dictionary<string, int> _NameToIndex = new Dictionary<string, int>();
    
    /// <summary>
    ///     Construct a empty name table
    /// </summary>
    public NameTable()
    {
    }

    public int FindIndex(string name)
    {
        if (_NameToIndex.Count == 0)
        {
            for (int index = 0; index < this.Count; index++)
            {
                NameTableItem item = this[index];
                _NameToIndex[item.Name] = index;
            }
        }

        return _NameToIndex.GetValueOrDefault(name, -1);
    }

    public void AddName(NameTableItem item)
    {
        Add(item);
        _NameToIndex[item.Name] = Count - 1;
    }
    

    /// <summary>
    ///     Uses a given serializer to deserialize the names in the stream
    /// </summary>
    /// <param name="serializer"></param>
    /// <param name="stream"></param>
    /// <param name="nameCount"></param>
    public NameTable(IStreamSerializer<NameTableItem> serializer, Stream stream, int nameCount)
    {
        serializer.ReadTArrayToList(stream, this, nameCount);
        //AddRange(serializer.ReadTArray(stream, nameCount));
    }
}

/// <summary>
///     A NameTableItem contains a name as a string and some related flag data. I don't know what the flags are used for
/// </summary>
[DebuggerDisplay("{Name}")]
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