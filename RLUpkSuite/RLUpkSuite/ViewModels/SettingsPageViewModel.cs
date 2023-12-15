using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using RLUpkSuite.Config;
using RLUpkSuite.Pages;

namespace RLUpkSuite.ViewModels;

public class SettingsPageViewModel : PageBase
{
    private readonly CommonConfig _commonConfig;

    private readonly ShellConfig _shellConfig;


    public SettingsPageViewModel(CommonConfig commonConfig, ShellConfig shellConfig) : base("Settings",
        PackIconKind.Cog)
    {
        _commonConfig = commonConfig;
        _shellConfig = shellConfig;
    }


    public BaseTheme BaseTheme
    {
        get => _shellConfig.BaseTheme;
        set
        {
            if (SetProperty(_shellConfig.BaseTheme, value, _shellConfig,
                    (config, newValue) => config.BaseTheme = newValue)) OnBaseThemeChanged(_shellConfig.BaseTheme);
        }
    }


    public PrimaryColor PrimaryColor
    {
        get => _shellConfig.PrimaryColor;
        set
        {
            if (SetProperty(_shellConfig.PrimaryColor, value, _shellConfig,
                    (config, color) => config.PrimaryColor = color)) OnPrimaryColorChanged(_shellConfig.PrimaryColor);
        }
    }

    public SecondaryColor SecondaryColor
    {
        get => _shellConfig.SecondaryColor;
        set
        {
            if (SetProperty(_shellConfig.SecondaryColor, value, _shellConfig,
                    (config, color) => config.SecondaryColor = color))
                OnSecondaryColorChanged(_shellConfig.SecondaryColor);
        }
    }

    private static void OnPrimaryColorChanged(PrimaryColor value)
    {
        var color = SwatchHelper.Lookup[(MaterialDesignColor)value];
        ChangePrimaryColor(color);
    }

    private void OnSecondaryColorChanged(SecondaryColor value)
    {
        var color = SwatchHelper.Lookup[(MaterialDesignColor)value];
        ChangeSecondaryColor(color);
    }

    private static void OnBaseThemeChanged(BaseTheme shellConfigBaseTheme)
    {
        PaletteHelper paletteHelper = new();
        var theme = paletteHelper.GetTheme();
        theme.SetBaseTheme(shellConfigBaseTheme);
        paletteHelper.SetTheme(theme);
    }

    private static void ChangePrimaryColor(Color color)
    {
        PaletteHelper paletteHelper = new();
        var theme = paletteHelper.GetTheme();
        theme.SetPrimaryColor(color);
        paletteHelper.SetTheme(theme);
    }

    public static void ChangeSecondaryColor(Color color)
    {
        PaletteHelper paletteHelper = new();
        var theme = paletteHelper.GetTheme();
        theme.SetSecondaryColor(color);
        paletteHelper.SetTheme(theme);
    }
}