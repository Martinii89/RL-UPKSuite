using System.Windows.Controls;
using PackageExplorer.ViewModel;

namespace PackageExplorer.View;

/// <summary>
///     Interaction logic for PackageView.xaml
/// </summary>
public partial class PackageUserControl : UserControl
{
    public PackageUserControl()
    {
        DataContext = new PackageUserControlViewModel();
        InitializeComponent();
    }
}