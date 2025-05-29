using System.Text.Json.Nodes;

namespace RlUpk.RLUpkSuite.Config;

public class CommonConfig : AppConfigBase
{
    public string? KeysPath { get; set; }

    public bool OpenOutputOnFinish { get; set; }

    public override string GetKey()
    {
        return "Common";
    }

    public override void UpdateFromConfig(JsonObject? jsonObject)
    {
        if (jsonObject is null)
        {
            return;
        }

        if (jsonObject.ContainsKey(nameof(KeysPath)))
        {
            KeysPath = jsonObject[nameof(KeysPath)]?.GetValue<string?>();
        }

        if (jsonObject.ContainsKey(nameof(OpenOutputOnFinish)))
        {
            OpenOutputOnFinish = jsonObject[nameof(OpenOutputOnFinish)]!.GetValue<bool>();
        }
    }
}