namespace RlUpk.Core.Types.PackageTables;

/// <summary>
///     Common data for items in the import and export table
/// </summary>
public interface IObjectResource
{
    /// <summary>
    ///     The Name of the object as a NameTable reference
    /// </summary>
    FName ObjectName { get; set; }

    /// <summary>
    ///     A reference to the outer object. Zero indicates that this is a top level object
    /// </summary>
    ObjectIndex OuterIndex { get; set; }
}