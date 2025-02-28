﻿using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ToraConHelper.Services;

public interface ISettingFileMamager
{
    Settings Load();
    void Save(Settings settings);
}

public class SettingFileManager : ISettingFileMamager
{
    private const string SettingFileName = "ToraCon-Helper_Settings.json";

    private static readonly JsonSerializerOptions jsonSerializeOptions = new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
        }
    };

    private string FilePath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SettingFileName);

    public Settings Load()
    {
        if (!File.Exists(FilePath))
        {
            return new Settings();
        }
        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<Settings>(json, jsonSerializeOptions)!;
    }

    public void Save(Settings settings)
    {
        var json = JsonSerializer.Serialize(settings, jsonSerializeOptions);
        File.WriteAllText(FilePath, json);
    }
}
