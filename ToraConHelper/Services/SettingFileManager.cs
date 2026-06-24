using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ToraConHelper.Services;

public interface ISettingFileManager
{
    Settings Load();
    void Save(Settings settings);
}

public class SettingFileManager : ISettingFileManager
{
    private const string SettingFileName = "ToraCon-Helper_Settings.json";
    private const int CurrentSettingsVersion = Settings.CurrentSchemeVersion;

    private static readonly JsonSerializerOptions jsonSerializeOptions = new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
        }
    };

    private string FilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ToraCon-Helper", SettingFileName);

    public Settings Load()
    {
        if (!File.Exists(FilePath))
        {
            return new Settings();
        }
        var json = File.ReadAllText(FilePath);
        var settings = JsonSerializer.Deserialize<Settings>(json, jsonSerializeOptions)!;

        // Migration v1 -> v2
        if (settings.SchemeVersion < CurrentSettingsVersion)
        {
            var migratedv2Json = Migratejson_v1_to_v2(json);
            settings = JsonSerializer.Deserialize<Settings>(migratedv2Json, jsonSerializeOptions)!;
            Save(settings); // バージョンアップした設定はすぐに保存しておく
        }
        return settings;
    }

    public void Save(Settings settings)
    {
        settings.SchemeVersion = CurrentSettingsVersion;
        if (!Directory.Exists(Path.GetDirectoryName(FilePath))) Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
        var json = JsonSerializer.Serialize(settings, jsonSerializeOptions);
        File.WriteAllText(FilePath, json);
    }

    #region Migration v1 -> v2

    // v1 -> v2 で変更されたプロパティのマップ。
    private static readonly Dictionary<string, string> V1LegacyPropertyNameMap = new Dictionary<string, string>
    {
        ["ReterderAllReduceActionEnabled"] = "RetarderAllReduceActionEnabled",
        ["ReterderAllReduceActionLimitSpeedKph"] = "RetarderAllReduceActionLimitSpeedKph",
        ["ReterderFullOnActionEnabled"] = "RetarderFullOnActionEnabled",
        ["ReterderFullOffActionEnabled"] = "RetarderFullOffActionEnabled",
        ["ReterderAutoOffActionEnabled"] = "RetarderAutoOffActionEnabled",
        ["ReterderAutoOffActionLimitSpeedKph"] = "RetarderAutoOffActionLimitSpeedKph",
        ["ReterderSkipInputActionEnabled"] = "RetarderSkipInputActionEnabled",
        ["ReterderSkipInputLevel"] = "RetarderSkipInputLevel",
        ["ReterderAllReduceOnThrottleEnabled"] = "RetarderAllReduceOnThrottleEnabled",
    };


    private static string Migratejson_v1_to_v2(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return json;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return json;
            }

            if (TryGetSettingsVersion(document.RootElement, out var version) && version >= CurrentSettingsVersion)
            {
                return json;
            }

            var migratedProperties = new Dictionary<string, JsonElement>();
            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (V1LegacyPropertyNameMap.TryGetValue(property.Name, out var newPropertyName))
                {
                    if (!migratedProperties.ContainsKey(newPropertyName))
                    {
                        migratedProperties[newPropertyName] = property.Value.Clone();
                    }
                }
                else
                {
                    migratedProperties[property.Name] = property.Value.Clone();
                }
            }

            migratedProperties["Version"] = JsonSerializer.SerializeToElement(CurrentSettingsVersion);
            return JsonSerializer.Serialize(migratedProperties);
        }
        catch (JsonException)
        {
            return json;
        }
    }

    private static bool TryGetSettingsVersion(JsonElement rootElement, out int version)
    {
        version = 0;
        if (!rootElement.TryGetProperty("Version", out var versionProperty) ||
            versionProperty.ValueKind != JsonValueKind.Number ||
            !versionProperty.TryGetInt32(out var parsedVersion))
        {
            return false;
        }

        version = parsedVersion;
        return true;
    }

    #endregion
}
