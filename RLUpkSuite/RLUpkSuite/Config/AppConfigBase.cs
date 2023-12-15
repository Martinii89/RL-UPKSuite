using System.Text.Json.Nodes;

namespace RLUpkSuite.Config;

public abstract class AppConfigBase
{
    public abstract string GetKey();

    /// <summary>
    ///     Update your own values from the json string
    /// </summary>
    /// <param name="jsonObject"></param>
    public abstract void UpdateFromConfig(JsonObject? jsonObject);
}