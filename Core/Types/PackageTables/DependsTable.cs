using Core.Serialization;

namespace Core.Types.PackageTables;

/// <summary>
///     A Depends table contains a array of ints for every exported item.
///     The ints are object indexes (just imports maybe?) to objects the export depends on.
///     This table is not present in cooked packages
/// </summary>
public class DependsTable : IBinaryDeserializableClass
{
    /// <summary>
    ///     A Array of ints for every export
    /// </summary>
    public TArray<TArray<int>> Depends { get; set; } = new();

    /// <summary>
    ///     Iterates the arrays and deserializes them.
    ///     You have to call InitializeSize first to set the number of arrays, because the number of arrays is not stored
    ///     together with the depends table serial data.
    /// </summary>
    /// <param name="reader"></param>
    public void Deserialize(Stream reader)
    {
        foreach (var depend in Depends)
        {
            depend.Deserialize(reader);
        }
    }

    /// <summary>
    ///     Set the number of arrays. This will most likely always just be the number of exports.
    /// </summary>
    /// <param name="exportCount"></param>
    public void InitializeSize(int exportCount)
    {
        for (var i = 0; i < exportCount; i++)
        {
            Depends.Add(new TArray<int>());
        }
    }
}