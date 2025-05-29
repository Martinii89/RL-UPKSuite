using RlUpk.Core.Types;
using RlUpk.Core.Types.PackageTables;

namespace RlUpk.Core.Utility.Export.Filters;

public class UnreversedClassFilter : IObjectFilter
{
    private readonly HashSet<string> _classesToIgnore = new(StringComparer.CurrentCultureIgnoreCase)
    {
        "AnimNodeAimOffset"
    };

    /// <inheritdoc />
    public bool ShouldRemove(UnrealPackage package, ImportTableItem importTableItem)
    {
        return false;
    }

    /// <inheritdoc />
    public bool ShouldRemove(UnrealPackage package, ExportTableItem exportTableItem)
    {
        var uObject = exportTableItem.Object;
        var className = uObject?.Class?.Name;
        return className != null && _classesToIgnore.Contains(className);
    }
}