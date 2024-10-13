using Gameloop.Vdf;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ToraConHelper.Helpers;

public static partial class SteamHelper
{
    const string BinaryPath = "bin\\win_x64";
    static readonly Dictionary<GameType, string> GameTypeAndIds = new() { { GameType.ETS2, "227300" }, { GameType.ATS, "270880" } };
    static readonly Dictionary<string, string> GameIds = new() {
        { /*ETS2*/"227300", "steamapps\\common\\Euro Truck Simulator 2" },
        { /*ATS*/"270880", "steamapps\\common\\American Truck Simulator" }
    };

    public static string? GetSteamPath()
    {
        var steamKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
        return steamKey?.GetValue("SteamPath") as string;
    }

    public static IEnumerable<(string Id, string Path)> DetectGamePaths()
    {
        try
        {
            var vdfFile = Path.Combine(SteamHelper.GetSteamPath(), "steamapps\\libraryfolders.vdf");
            dynamic vdf = VdfConvert.Deserialize(File.ReadAllText(vdfFile));

            var result = new List<(string Id, string Path)>();
            foreach (dynamic content in vdf.Value)
            {
                var basePath = content.Value.path.Value as string;
                var apps = content.Value.apps;
                foreach (dynamic app in apps)
                {
                    var detectedGames = GameIds.Where(id => id.Key == app.Key as string)
                                               .Select(id => Path.Combine(basePath, id.Value));
                    foreach (var game in detectedGames) result.Add(new() { Id = app.Key, Path = game });
                }
            }
            return result;
        }
        catch
        {
            return [];
        }
    }

    public static string GetExePath(string gamePath) => Path.Combine(gamePath, BinaryPath);

    public static string? GetGameFolder(GameType gameType)
    {
        var path = DetectGamePaths().Where(gp => gp.Id == GameTypeAndIds[gameType]).Select(gp => gp.Path).FirstOrDefault();
        return path;
    }
}
