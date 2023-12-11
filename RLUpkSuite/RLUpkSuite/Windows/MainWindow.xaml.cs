using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;

using MaterialDesignThemes.Wpf;

using RLUpkSuite.ViewModels;

namespace RLUpkSuite.Windows
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = _viewModel = viewModel;
            InitializeComponent();
            UseDarkTheme.IsChecked = Theme.GetSystemTheme() == BaseTheme.Dark;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _viewModel.SaveConfig();
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
}