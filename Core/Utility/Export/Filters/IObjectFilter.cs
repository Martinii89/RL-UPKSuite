using RlUpk.Core.Types;
using RlUpk.Core.Types.PackageTables;

namespace RlUpk.Core.Utility.Export.Filters;

/// <summary>
///     A IObjectFilter should have the responsibility of deciding if a object should be included in a package export or
///     not
/// </summary>
public interface IObjectFilter
{
    /// <summary>
    ///     This method should return true if a object should NOT be included in the package export
    /// </summary>
    /// <param name="package">The owner package</param>
    /// <param name="importTableItem">The import object checked for removal</param>
    /// <returns></returns>
    bool ShouldRemove(UnrealPackage package, ImportTableItem importTableItem);

    /// <summary>
    ///     This method should return true if a object should NOT be included in the package export
    /// </summary>
    /// <param name="package">The owner package</param>
    /// <param name="exportTableItem">The import object checked for removal</param>
    /// <returns></returns>
    bool ShouldRemove(UnrealPackage package, ExportTableItem exportTableItem);
}