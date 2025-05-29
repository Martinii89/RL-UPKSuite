using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;

using MaterialDesignColors;

using MaterialDesignThemes.Wpf;

using RlUpk.RLUpkSuite.Config;
using RlUpk.RLUpkSuite.Pages;
using RlUpk.RLUpkSuite.ViewModels;

namespace RlUpk.RLUpkSuite.Windows;

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
            string str = File.ReadAllText(path);
            _configStore.Load(str);
        }

        ObservableCollection<PageBase> viewModelPages = _viewModel.Pages;
        _viewModel.SelectedPage = viewModelPages.FirstOrDefault(x => x.PageName == _shellConfig.StartPage) ??
                                  viewModelPages.FirstOrDefault();
        InitThemeFromConfig();
    }

    private void InitStartPageFromConfig()
    {
    }

    private void InitThemeFromConfig()
    {
        PaletteHelper paletteHelper = new();
        Theme theme = paletteHelper.GetTheme();
        Color primary = SwatchHelper.Lookup[(MaterialDesignColor)_shellConfig.PrimaryColor];
        Color secondary = SwatchHelper.Lookup[(MaterialDesignColor)_shellConfig.SecondaryColor];

        theme.SetBaseTheme(_shellConfig.BaseTheme);
        theme.SetPrimaryColor(primary);
        theme.SetSecondaryColor(secondary);
        paletteHelper.SetTheme(theme);
    }

    private void WriteConfig()
    {
        string config = _configStore.Export();
        string directory = Path.GetDirectoryName(_configPath) ??
                           throw new InvalidOperationException($"Failed to resolve directory from {_configPath}");
        Directory.CreateDirectory(directory);
        File.WriteAllText(_configPath, config);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        WriteConfig();
        base.OnClosing(e);
    }
}