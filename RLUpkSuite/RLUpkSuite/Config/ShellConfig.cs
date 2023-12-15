using System.Text.Json.Nodes;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace RLUpkSuite.Config;

// ReSharper disable once ClassNeverInstantiated.Global
public class ShellConfig : AppConfigBase
{
    public string? StartPage { get; set; }

    public BaseTheme BaseTheme { get; set; } = BaseTheme.Inherit;

    public PrimaryColor PrimaryColor { get; set; } = PrimaryColor.Blue;
    public SecondaryColor SecondaryColor { get; set; } = SecondaryColor.LightBlue;

    public override string GetKey()
    {
        return "Shell";
    }

    public override void UpdateFromConfig(JsonObject? jsonObject)
    {
        if (jsonObject is null) return;

        if (jsonObject.ContainsKey(nameof(StartPage))) StartPage = jsonObject[nameof(StartPage)]?.GetValue<string?>();

        if (jsonObject.ContainsKey(nameof(PrimaryColor)))
            PrimaryColor = (PrimaryColor)jsonObject[nameof(PrimaryColor)]!.GetValue<int>();

        if (jsonObject.ContainsKey(nameof(SecondaryColor)))
            SecondaryColor = (SecondaryColor)jsonObject[nameof(SecondaryColor)]!.GetValue<int>();

        if (jsonObject.ContainsKey(nameof(BaseTheme)))
            BaseTheme = (BaseTheme)jsonObject[nameof(BaseTheme)]!.GetValue<int>();
    }
}