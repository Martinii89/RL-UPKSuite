using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Decryptor.Wpf.MVVM.ViewModel;
using ModernWpf;

namespace Decryptor.Wpf;


public partial class MainWindow : Window
{

    private readonly MainWindowViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        DataContext = _viewModel;

    }

    private void MyCanvas_OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            // Note that you can have more than one file.
            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);

            // Assuming you have one file that you care about, pass it off to whatever
            // handling code you have defined.
        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var themeManager = ThemeManager.Current;
        if (themeManager.ApplicationTheme == ApplicationTheme.Dark)
        {
            themeManager.ApplicationTheme = ApplicationTheme.Light;
        }
        else
        {
            themeManager.ApplicationTheme = ApplicationTheme.Dark;
        }
    }
}
