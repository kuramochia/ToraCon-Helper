using System;
using System.IO;
using System.Text;

namespace ToraConHelper.Helpers;

internal static class PowerToysHelper
{
    internal static string GetDefaultGameDataFolder(GameType gameType)
    {
        switch (gameType)
        {
            case GameType.ETS2:
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Euro Truck Simulator 2");
            case GameType.ATS:
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "American Truck Simulator");
        }
        return string.Empty;
    }

    internal static Uri? GetGameRunUri(GameType gameType)
    {
        switch (gameType)
        {
            case GameType.ETS2:
                return new Uri("steam://run/227300");
            case GameType.ATS:
                return new Uri("steam://run/270880");
        }
        return null;
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
