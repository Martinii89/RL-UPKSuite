using System.Text.Json.Nodes;

namespace RLUpkSuite.Config;

[Serializable]
public class DecryptionConfig : AppConfigBase
{
    public bool ShowOutputDirectory { get; set; }
    public string? OutputDirectory { get; set; }

    public override string GetKey()
    {
        return "Decryption";
    }

    public override void UpdateFromConfig(JsonObject? jsonObject)
    {
        if (jsonObject is null) return;

        if (jsonObject.ContainsKey(nameof(ShowOutputDirectory)))
            ShowOutputDirectory = jsonObject[nameof(ShowOutputDirectory)]!.GetValue<bool>();

        if (jsonObject.ContainsKey(nameof(OutputDirectory)))
            OutputDirectory = jsonObject[nameof(OutputDirectory)]?.GetValue<string>();
    }
}