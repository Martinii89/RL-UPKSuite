using System.Text.Json.Nodes;
using System.Windows.Media;

namespace RLUpkSuite.Config;

// ReSharper disable once ClassNeverInstantiated.Global
public class ShellConfig : AppConfig
{
    public string? StartPage { get; set; }

    public Color? PrimaryColor { get; set; }

    public override string GetKey()
    {
        return "Shell";
    }

    public override void UpdateFromConfig(JsonObject? jsonObject)
    {
        if (jsonObject is null)
        {
            return;
        }

        if (jsonObject.ContainsKey(nameof(StartPage)))
        {
            StartPage = jsonObject[nameof(StartPage)]?.GetValue<string?>();
        }
        
        if (jsonObject.ContainsKey(nameof(PrimaryColor)))
        {
            PrimaryColor = jsonObject[nameof(PrimaryColor)]?.GetValue<Color?>();
        }
    }
}