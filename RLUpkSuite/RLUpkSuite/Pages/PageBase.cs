using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using MaterialDesignThemes.Wpf;

namespace RlUpk.RLUpkSuite.Pages;

public abstract partial class PageBase : ObservableObject
{
    [ObservableProperty]
    private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement = ScrollBarVisibility.Auto;

    [ObservableProperty]
    private Thickness _marginRequirement = new(8, 8, 4, 4);

    [ObservableProperty]
    private PackIconKind _pageIcon;

    [ObservableProperty]
    private string _pageName;

    [ObservableProperty]
    private ScrollBarVisibility _verticalScrollBarVisibilityRequirement = ScrollBarVisibility.Auto;

    /// <inheritdoc />
    protected PageBase(string pageName, PackIconKind pageIcon)
    {
        _pageName = pageName;
        _pageIcon = pageIcon;
    }
}