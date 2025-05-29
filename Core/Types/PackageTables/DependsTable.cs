namespace RlUpk.Core.Types.PackageTables;

/// <summary>
///     A Depends table contains a array of ints for every exported item.
///     The ints are object indexes (just imports maybe?) to objects the export depends on.
///     This table is not present in cooked packages
/// </summary>
public class DependsTable
{
    /// <summary>
    ///     A Array of ints for every export
    /// </summary>
    public List<List<int>> Depends { get; set; } = new();
}