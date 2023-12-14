using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;

using MaterialDesignThemes.Wpf;

using RLUpkSuite.Config;
using RLUpkSuite.Pages;

namespace RLUpkSuite.ViewModels;

public partial class SettingsPageViewModel : PageBase
{
    private readonly CommonConfig _commonConfig;
    private readonly ShellConfig _shellConfig;
    //
    // [ObservableProperty]
    // private Color _primaryColor;


    public Color? PrimaryColor
    {
        get => _shellConfig.PrimaryColor;
        set
        {
            if (SetProperty(_shellConfig.PrimaryColor, value, _shellConfig, (config, color) => config.PrimaryColor = color))
            {
                OnPrimaryColorChanged(_shellConfig.PrimaryColor);
            }
        }
    }


    public SettingsPageViewModel(CommonConfig commonConfig, ShellConfig shellConfig) : base("Settings", PackIconKind.Cog)
    {
        _commonConfig = commonConfig;
        _shellConfig = shellConfig;
        var paletteHelper = new PaletteHelper();
        Theme theme = paletteHelper.GetTheme();
        PrimaryColor = theme.PrimaryMid.Color;
    }

    private void OnPrimaryColorChanged(Color? value)
    {
        if (value.HasValue)
        {
            ChangePrimaryColor(value.Value);
        }
    }

    public static void ChangePrimaryColor(Color color)
    {
        var paletteHelper = new PaletteHelper();
        Theme theme = paletteHelper.GetTheme();

        theme.PrimaryLight = new ColorPair(color.Lighten());
        theme.PrimaryMid = new ColorPair(color);
        theme.PrimaryDark = new ColorPair(color.Darken());

        paletteHelper.SetTheme(theme);
    }

    public static void ChangeSecondaryColor(Color color)
    {
        var paletteHelper = new PaletteHelper();
        Theme theme = paletteHelper.GetTheme();

        theme.SecondaryLight = new ColorPair(color.Lighten());
        theme.SecondaryMid = new ColorPair(color);
        theme.SecondaryDark = new ColorPair(color.Darken());

        paletteHelper.SetTheme(theme);
    }
}