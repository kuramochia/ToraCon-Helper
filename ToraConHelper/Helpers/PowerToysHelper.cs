using System;
using System.IO;
using System.Text;

namespace ToraConHelper.Helpers;

internal static class PowerToysHelper
{
    internal static string GetDefaultGameDataFolder(GameType gameType)
    {
        return gameType switch
        {
            GameType.ETS2 => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Euro Truck Simulator 2"),
            GameType.ATS => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "American Truck Simulator"),
            _ => string.Empty,
        };
    }

    internal static Uri? GetGameRunUri(GameType gameType)
    {
        return gameType switch
        {
            GameType.ETS2 => new Uri("steam://run/227300"),
            GameType.ATS => new Uri("steam://run/270880"),
            _ => null,
        };
    }

    internal static string ConvertHexProfileNameToString(string hex)
    {
        int length = hex.Length;
        byte[] bytes = new byte[length / 2];
        for (int i = 0; i < length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return new UTF8Encoding().GetString(bytes);
    }

}
