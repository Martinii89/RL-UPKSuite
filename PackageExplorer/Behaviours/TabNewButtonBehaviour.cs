using Microsoft.Xaml.Behaviors;
using PackageExplorer.ViewModel;
using Syncfusion.Windows.Tools.Controls;

namespace PackageExplorer;

public class TabNewButtonBehaviour : Behavior<TabControlExt>
{
    protected override void OnAttached()
    {
        var vm = AssociatedObject.DataContext as MainWindowViewModel;
        AssociatedObject.NewButtonClick += (sender, args) => { vm?.OpenFileDialogCommand.Execute(null); };
    }
}