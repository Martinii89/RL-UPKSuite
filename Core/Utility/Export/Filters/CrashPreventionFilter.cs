using RlUpk.Core.Classes.Core;
using RlUpk.Core.Types;
using RlUpk.Core.Types.PackageTables;

namespace RlUpk.Core.Utility.Export.Filters;

/// <summary>
///     A CrashPreventionFilter will remove any objects that are known to crash udk if included in the export
/// </summary>
public class CrashPreventionFilter : IObjectFilter
{
    /// <inheritdoc />
    public bool ShouldRemove(UnrealPackage package, ImportTableItem importTableItem)
    {
        if (IsNullImport(package, importTableItem))
        {
            return true;
        }

        if (importTableItem.ImportedObject is null)
        {
            return true;
        }

        if (IsInternalImport(package, importTableItem.ImportedObject))
        {
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public bool ShouldRemove(UnrealPackage package, ExportTableItem exportTableItem)
    {
        return IsNullExport(package, exportTableItem) || exportTableItem.Object.Name.StartsWith("Default__");
    }

    /// <summary>
    ///     Objects with None as ObjectName, className and packageName are invalid and should be removed
    /// </summary>
    /// <param name="package"></param>
    /// <param name="import"></param>
    /// <returns></returns>
    private static bool IsNullImport(UnrealPackage package, ImportTableItem import)
    {
        if (package.GetName(import.ObjectName) != "None")
        {
            return false;
        }

        if (package.GetName(import.ClassName) != "None")
        {
            return false;
        }

        if (package.GetName(import.ClassPackage) != "None")
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Exports with a zero serial size are invalid and should be removed
    /// </summary>
    /// <param name="package"></param>
    /// <param name="exportTableItem"></param>
    /// <returns></returns>
    private static bool IsNullExport(UnrealPackage package, ExportTableItem exportTableItem)
    {
        return exportTableItem.SerialSize == 0;
    }


    /// <summary>
    ///     Having imported objects within exported objects seems to always crash UDK
    /// </summary>
    /// <param name="package"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    private static bool IsInternalImport(UnrealPackage package, UObject obj)
    {
        return obj.Outer?.OwnerPackage == package && obj.Outer?.ExportTableItem is not null;
    }
}