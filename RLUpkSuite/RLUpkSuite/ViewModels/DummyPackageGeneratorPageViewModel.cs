using MaterialDesignThemes.Wpf;

using RLUpkSuite.Pages;

namespace RLUpkSuite.ViewModels
{
    public class DummyPackageGeneratorPageViewModel : PageBase
    {
        public DummyPackageGeneratorPageViewModel() : base("Generate", PackIconKind.Pencil)
        {
        }
    }

    public class SettingsPageViewModel : PageBase
    {
        public SettingsPageViewModel() : base("Settings", PackIconKind.Cog)
        {
        }
    }
}