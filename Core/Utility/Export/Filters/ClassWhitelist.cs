using Core.Types;
using Core.Types.PackageTables;

namespace Core.Utility.Export.Filters;

public class ClassWhitelist : IObjectFilter
{
    private readonly HashSet<string> _classWhitelist = new(StringComparer.CurrentCultureIgnoreCase)
    {
        "Texture2D", "Package"
    };


    /// <inheritdoc />
    public bool ShouldRemove(UnrealPackage package, ImportTableItem importTableItem)
    {
        return false;
    }

    /// <inheritdoc />
    public bool ShouldRemove(UnrealPackage package, ExportTableItem exportTableItem)
    {
        var objClassName = exportTableItem.Object?.Class?.Name;
        return !_classWhitelist.Contains(objClassName);
    }
}