using RlUpk.Core.Types;
using RlUpk.Core.Types.PackageTables;

namespace RlUpk.Core.Utility.Export.Filters;

internal class ExportIndexFilter : IObjectFilter
{
    private readonly int _indexMax;
    private readonly int _indexMin;

    public ExportIndexFilter(int min, int max)
    {
        _indexMin = min;
        _indexMax = max;
    }

    public bool ShouldRemove(UnrealPackage package, ImportTableItem importTableItem)
    {
        return false;
    }

    public bool ShouldRemove(UnrealPackage package, ExportTableItem exportTableItem)
    {
        var obj = package.ExportTable.FirstOrDefault(x => x.Object == exportTableItem.Object);
        var indexOf = package.ExportTable.IndexOf(obj);
        return indexOf < _indexMin || indexOf > _indexMax;
    }
}