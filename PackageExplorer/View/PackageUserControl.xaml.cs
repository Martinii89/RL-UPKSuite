using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Core.Classes.Core;
using Syncfusion.Windows.PropertyGrid;

namespace PackageExplorer.View;

/// <summary>
///     Interaction logic for PackageView.xaml
/// </summary>
public partial class PackageUserControl : UserControl
{
    private static PropertyInfo? _parentPropertyItemPropertyInfo;

    public PackageUserControl()
    {
        //DataContext = new PackageUserControlViewModel();
        InitializeComponent();

        _parentPropertyItemPropertyInfo = typeof(PropertyItem).GetProperty("ParentPropertyItem", BindingFlags.Instance |
                                                                                                 BindingFlags.Public |
                                                                                                 BindingFlags.NonPublic);
    }

    private void PropertyGrid_OnAutoGeneratingPropertyGridItem(object? sender, AutoGeneratingPropertyGridItemEventArgs e)
    {
        var grid = sender as PropertyGrid;
        var src = e.OriginalSource as PropertyItem;
        var parent = _parentPropertyItemPropertyInfo?.GetValue(src, null) as PropertyItem;
        if (parent?.Value is UObject uObject)
        {
            uObject.Deserialize();
        }
    }

    private void PropertyGrid_OnSelectedPropertyItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }
}