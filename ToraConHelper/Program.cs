using System;
using System.IO;
using System.Runtime;
using System.Threading;
using ToraConHelper.Installer;

namespace ToraConHelper;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length == 1 && string.Equals("-install", args[0], StringComparison.OrdinalIgnoreCase))
        {
            var pluginApp = new PluginApp();
            pluginApp.Run();
        }
        else
        {
            // start ProfileOptimization ( Multicore JIT )
            ProfileOptimization.SetProfileRoot(Path.GetTempPath());
            ProfileOptimization.StartProfile("ToraConHelper.JIT.profile");

            // 多重起動防止用の名前付きイベント
            using EventWaitHandle singleInstanceEvent = new(
                false,
                EventResetMode.AutoReset,
                "ToraConHelper",
                out var createdNew);

            if (createdNew)
            {
                // 新規起動
                var app = new App
                {
                    SingleInstanceEvent = singleInstanceEvent,
                };
                app.Run();
            }
            else
            {
                // 既存のウィンドウを表示
                singleInstanceEvent.Set();
            }
        }
    }
}
