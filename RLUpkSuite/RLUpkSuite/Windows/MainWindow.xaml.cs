using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;

using MaterialDesignThemes.Wpf;

using RLUpkSuite.Config;
using RLUpkSuite.ViewModels;

namespace RLUpkSuite.Windows;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly AppConfigStore _configStore;
    private readonly MainWindowViewModel _viewModel;
    private string _configPath;

    public MainWindow(MainWindowViewModel viewModel, AppConfigStore configStore)
    {
        _configStore = configStore;
        DataContext = _viewModel = viewModel;
        InitializeComponent();
        UseDarkTheme.IsChecked = Theme.GetSystemTheme() == BaseTheme.Dark;
    }

    public void InitConfig(string path)
    {
        _configPath = path;
        if (File.Exists(path))
        {
            string str = File.ReadAllText(path);
            _configStore.Load(str);
        }
    }

    private void WriteConfig()
    {
        string config = _configStore.Export();
        string directory = Path.GetDirectoryName(_configPath) ?? throw new InvalidOperationException($"Failed to resolve directory from {_configPath}");
        Directory.CreateDirectory(directory);
        File.WriteAllText(_configPath, config);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        WriteConfig();
        base.OnClosing(e);
    }

    private void ThemeToggleChanged(object sender, RoutedEventArgs e)
    {
        bool? useDarkTheme = ((ToggleButton)sender).IsChecked;
        if (useDarkTheme is null)
        {
            return;
        }

        PaletteHelper paletteHelper = new();
        //Retrieve the app's existing theme
        Theme theme = paletteHelper.GetTheme();
        theme.SetBaseTheme(useDarkTheme.Value ? BaseTheme.Dark : BaseTheme.Light);
        paletteHelper.SetTheme(theme);
    }
}