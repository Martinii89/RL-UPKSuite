using Core.Types.PackageTables;

namespace Core.Types;

/// <summary>
///     A name stored as a index and a instance number. the index maps to a item in the <see cref="NameTable" /> of the
///     package
/// </summary>
public readonly struct FName
{
    /// <summary>
    ///     Default constructor.
    ///     Pretty worthless on it's own. Required for default construction before calling Deserialize.
    ///     May be removed for a better deserialization system later.
    /// </summary>
    public FName()
    {
        NameIndex = 0;
        InstanceNumber = 0;
    }

    /// <summary>
    ///     Construct a FName from a given index and instance number.
    /// </summary>
    /// <param name="nameIndex"></param>
    /// <param name="instanceNumber"></param>
    public FName(int nameIndex, int instanceNumber = 0)
    {
        NameIndex = nameIndex;
        InstanceNumber = instanceNumber;
    }


    private sealed class NameIndexInstanceNumberEqualityComparer : IEqualityComparer<FName>
    {
        public bool Equals(FName x, FName y)
        {
            return x.NameIndex == y.NameIndex && x.InstanceNumber == y.InstanceNumber;
        }

        public int GetHashCode(FName obj)
        {
            return HashCode.Combine(obj.NameIndex, obj.InstanceNumber);
        }
    }

    public static IEqualityComparer<FName> NameIndexInstanceNumberComparer { get; } = new NameIndexInstanceNumberEqualityComparer();

    /// <summary>
    ///     The index in the <see cref="NameTable" />
    /// </summary>
    public int NameIndex { get; init; }

    /// <summary>
    ///     Instance number
    /// </summary>
    public int InstanceNumber { get; init; }
}