using Core.Classes.Core;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Utility.Export.Filters;

internal class ClassExportFilter : IObjectFilter
{
    public bool ShouldRemove(UnrealPackage package, ImportTableItem importTableItem)
    {
        return false;
    }

    public bool ShouldRemove(UnrealPackage package, ExportTableItem exportTableItem)
    {
        var obj = exportTableItem.Object;
        return obj?.Class == package.StaticClass;
    }
}