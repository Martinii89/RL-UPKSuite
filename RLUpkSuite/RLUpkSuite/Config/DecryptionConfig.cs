using System.Text.Json.Nodes;

namespace RLUpkSuite.Config;

public class DecryptionConfig(CommonConfig commonConfig) : AppConfigBase
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

    public string? OutputDirectory { get; set; }

    public override string GetKey()
    {
        return "Decryption";
    }

    public override void UpdateFromConfig(JsonObject? jsonObject)
    {
        if (jsonObject is null)
        {
            return;
        }

        if (jsonObject.ContainsKey(nameof(OutputDirectory)))
        {
            OutputDirectory = jsonObject[nameof(OutputDirectory)]?.GetValue<string>();
        }
    }
}