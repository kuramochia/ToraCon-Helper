using System.IO;
using System.Reflection;
using System.Text.Json;

namespace ToraConHelper.Services;

public interface ISettingFileMamager
{
    Settings Load();
    void Save(Settings settings);
}

public class SettingFileManager : ISettingFileMamager
{
    private string FilePath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\ToraCon-Helper_Settings.json";

    public SettingFileManager()
    {
    }

    public Settings Load()
    {
        if (!File.Exists(FilePath))
        {
            return new Settings();
        }
        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<Settings>(json)!;
    }

    private static readonly JsonSerializerOptions jsonSerializeOptions = new JsonSerializerOptions { WriteIndented = true };

    public void Save(Settings settings)
    {
        var json = JsonSerializer.Serialize(settings, jsonSerializeOptions);
        File.WriteAllText(FilePath, json);
    }
}


public class Settings
{
    public bool IsActive { get; set; } = true;
    public bool GoToTasktrayOnAppClose { get; set; } = false;
    public bool TaskTrayOnStart { get; set; } = false;

    public bool BlinkerLikeRealCarActionEnabled { get; set; } = true;
    public bool RetarderAllReduceActionEnabled { get; set; } = true;
    public int RetarderAllReduceActionLimitSpeedKph { get; set; } = 30;

    public bool BlinkerHideOnSteeringActionEnabled { get; set; } = false;

    public int SteeringRotationAngle { get; set; } = 1800;

    public int BlinkerHideBySteeringAngle { get; set; } = 90;

    public bool RetarderFullOnActionEnabled { get; set; } = false;
    public bool RetarderFullOffActionEnabled { get; set; } = false;
}