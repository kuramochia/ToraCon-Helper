using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ToraConHelper.Services;

public class ShortcutService
{
    private const string ShortcutFileName = "ToraConHelper.lnk";
    private static readonly string ExeFilePath = Assembly.GetEntryAssembly().Location;
    public bool Create()
    {
        if (HasShortcutFile()) return true;
        Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")); //Windows Script Host Shell Object
        dynamic shell = Activator.CreateInstance(t);
        try
        {
            var lnk = shell.CreateShortcut(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), ShortcutFileName));
            try
            {
                lnk.TargetPath = ExeFilePath;
                lnk.IconLocation = $"{ExeFilePath}, 0";
                lnk.Save();
                return true;
            }
            finally
            {
                Marshal.FinalReleaseComObject(lnk);
            }
        }
        catch
        {
            return false;
        }
        finally
        {
            Marshal.FinalReleaseComObject(shell);
        }
    }

    public bool Delete()
    {
        if (!HasShortcutFile()) return false;
        try
        {
            File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), ShortcutFileName));
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool HasShortcutFile()
        => File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), ShortcutFileName));
}
