using Gameloop.Vdf;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ToraConHelper.Helpers;

public static partial class SteamHelper
{
    static readonly Dictionary<string, string> GameIds = new() {
        { /*ETS2*/"227300", "steamapps\\common\\Euro Truck Simulator 2" },
        { /*ATS*/"270880", "steamapps\\common\\American Truck Simulator" }
    };

    public static string? GetSteamPath()
    {
        var steamKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
        return steamKey?.GetValue("SteamPath") as string;
    }

    public static IEnumerable<string> DetectGamePaths()
    {
        try
        {
            var vdfFile = Path.Combine(SteamHelper.GetSteamPath(), "steamapps\\libraryfolders.vdf");
            dynamic vdf = VdfConvert.Deserialize(File.ReadAllText(vdfFile));

            var result = new List<string>();
            foreach (dynamic content in vdf.Value)
            {
                var basePath = content.Value.path.Value as string;
                var apps = content.Value.apps;
                foreach (dynamic app in apps)
                {
                    var detectedGames = GameIds.Where(id => id.Key == app.Key as string).Select(id => Path.Combine(basePath, id.Value));
                    result.AddRange(detectedGames);
                }
            }
            return result;
        }
        catch
        {
            return [];
        }
    }
}
