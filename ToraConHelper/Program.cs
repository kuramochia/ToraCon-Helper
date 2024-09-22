using System;
using System.Threading;
using ToraConHelper.Installer;

namespace ToraConHelper;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        if ( args.Length == 1 && string.Equals("-install", args[0],StringComparison.OrdinalIgnoreCase))
        {
            var pluginApp = new PluginApp();
            pluginApp.Run();
        }
        else
        {
            // 多重起動防止用 Mutex
            using Mutex mutex = new(true, "ToraConHelper", out var createdNew);
            try
            {
                if (createdNew)
                {
                    // 新規起動
                    var app = new App();
                    app.Run();
                }
            }
            finally
            {
                if (createdNew) mutex.ReleaseMutex();
            }
        }
    }
}
