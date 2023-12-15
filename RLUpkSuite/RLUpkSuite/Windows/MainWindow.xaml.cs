using System.ComponentModel;
using System.IO;
using MaterialDesignColors;
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

    private readonly ShellConfig _shellConfig;

    private readonly MainWindowViewModel _viewModel;

    private string _configPath;

    public MainWindow(MainWindowViewModel viewModel, AppConfigStore configStore, ShellConfig shellConfig)
    {
        _configStore = configStore;
        _shellConfig = shellConfig;
        DataContext = _viewModel = viewModel;
        InitializeComponent();
        // UseDarkTheme.IsChecked = Theme.GetSystemTheme() == BaseTheme.Dark;
    }

    public void InitConfig(string path)
    {
        _configPath = path;
        if (File.Exists(path))
        {
            var str = File.ReadAllText(path);
            _configStore.Load(str);
        }

        InitThemeFromConfig();
    }

    private void InitThemeFromConfig()
    {
        PaletteHelper paletteHelper = new();
        var theme = paletteHelper.GetTheme();
        var primary = SwatchHelper.Lookup[(MaterialDesignColor)_shellConfig.PrimaryColor];
        var secondary = SwatchHelper.Lookup[(MaterialDesignColor)_shellConfig.SecondaryColor];

        theme.SetBaseTheme(_shellConfig.BaseTheme);
        theme.SetPrimaryColor(primary);
        theme.SetSecondaryColor(secondary);
        paletteHelper.SetTheme(theme);
    }

    private void WriteConfig()
    {
        var config = _configStore.Export();
        var directory = Path.GetDirectoryName(_configPath) ??
                        throw new InvalidOperationException($"Failed to resolve directory from {_configPath}");
        Directory.CreateDirectory(directory);
        File.WriteAllText(_configPath, config);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        WriteConfig();
        base.OnClosing(e);
    }

    // private void ThemeToggleChanged(object sender, RoutedEventArgs e)
    // {
    //     bool? useDarkTheme = ((ToggleButton)sender).IsChecked;
    //     if (useDarkTheme is null)
    //     {
    //         return;
    //     }
    //
    //     PaletteHelper paletteHelper = new();
    //     //Retrieve the app's existing theme
    //     Theme theme = paletteHelper.GetTheme();
    //     theme.SetBaseTheme(useDarkTheme.Value ? BaseTheme.Dark : BaseTheme.Light);
    //     paletteHelper.SetTheme(theme);
    // }
}