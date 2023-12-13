using System.Text.Json.Nodes;

namespace RLUpkSuite.Config
{
    public class CommonConfig : AppConfig
    {
        public override string GetKey() => "Common";
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
        }

        public string? KeysPath { get; set; }
    }
}