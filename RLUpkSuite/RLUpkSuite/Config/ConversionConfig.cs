using System.Text.Json.Nodes;

namespace RLUpkSuite.Config;

public class ConversionConfig(CommonConfig commonConfig) : AppConfigBase
{
    public string? KeysPath
    {
        get => commonConfig.KeysPath;
        set => commonConfig.KeysPath = value;
    }

    public bool OpenOutputOnFinish
    {
        get => commonConfig.OpenOutputOnFinish;
        set => commonConfig.OpenOutputOnFinish = value;
    }

    public string? ImportPackagesDirectory { get; set; } = string.Empty;

    public string? OutputDirectory { get; set; } = string.Empty;

    public string? Suffix { get; set; } = string.Empty;

    public bool Compress { get; set; }


    public override string GetKey()
    {
        return "Converter";
    }

    public override void UpdateFromConfig(JsonObject? jsonObject)
    {
        if (jsonObject is null)
        {
            return;
        }

        if (jsonObject.ContainsKey(nameof(ImportPackagesDirectory)))
        {
            ImportPackagesDirectory = jsonObject[nameof(ImportPackagesDirectory)]?.GetValue<string>() ?? String.Empty;
        }

        if (jsonObject.ContainsKey(nameof(OutputDirectory)))
        {
            OutputDirectory = jsonObject[nameof(OutputDirectory)]?.GetValue<string>() ?? String.Empty;
        }

        if (jsonObject.ContainsKey(nameof(Suffix)))
        {
            Suffix = jsonObject[nameof(Suffix)]?.GetValue<string>() ?? String.Empty;
        }

        if (jsonObject.ContainsKey(nameof(Compress)))
        {
            Compress = jsonObject[nameof(Compress)]?.GetValue<bool>() ?? false;
        }
    }
}