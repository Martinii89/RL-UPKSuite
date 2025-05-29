using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Types;
using RlUpk.Core.Types.PackageTables;

namespace RlUpk.Core.Utility.Export.Filters;

/// <summary>
///     A MapObjectFilter will remove any objects that has a UWorld object in it's outer chain. It will also remove the
///     UWorld object itself
/// </summary>
public class MapObjectFilter : IObjectFilter
{
    /// <inheritdoc />
    public bool ShouldRemove(UnrealPackage package, ImportTableItem importTableItem)
    {
        return false;
    }

    /// <inheritdoc />
    public bool ShouldRemove(UnrealPackage package, ExportTableItem exportTableItem)
    {
        var uObject = exportTableItem.Object;
        if (uObject is UWorld)
        {
            return true;
        }

        var outers = uObject?.GetOuterEnumerable();
        if (outers is null)
        {
            return false;
        }

        if (outers.Any(x => x is UWorld))
        {
            return true;
        }

        return false;
    }
}