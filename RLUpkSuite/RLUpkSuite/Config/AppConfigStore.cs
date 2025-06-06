﻿using System.Text.Json;
using System.Text.Json.Nodes;

namespace RlUpk.RLUpkSuite.Config;

public class AppConfigStore
{
    private readonly Dictionary<string, AppConfigBase> _configSections;

    public AppConfigStore(IEnumerable<AppConfigBase> configs)
    {
        _configSections = configs.ToDictionary(x => x.GetKey(), x => x);
    }

    public string Export()
    {
        JsonObject jsonObject = new();
        foreach ((string key, AppConfigBase value) in _configSections)
        {
            Type type = value.GetType();
            string jsonNode = JsonSerializer.Serialize(value, type);
            jsonObject.Add(key, JsonNode.Parse(jsonNode));
        }

        return jsonObject.ToJsonString();
    }

    public void Load(string jsonText)
    {
        if (JsonNode.Parse(jsonText) is not JsonObject configs)
        {
            return;
        }

        foreach ((string key, AppConfigBase value) in _configSections)
        {
            if (!configs.ContainsKey(key))
            {
                continue;
            }

            value.UpdateFromConfig(configs[key] as JsonObject);

            // Type valueType = value.GetType();
            // string json = configs[key]!.ToJsonString();
            // object? deserialize = JsonSerializer.Deserialize(json, valueType);
            // if (deserialize is AppConfig configSection && configSection.GetKey() == key)
            // {
            //     _configSections[key] = configSection;
            // }
        }
    }
}