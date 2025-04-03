using System;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
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
            ProfileOptimization.SetProfileRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            ProfileOptimization.StartProfile("ToraConHelper.JIT.profile");

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
                else
                {
                    // 5秒であきらめる
                    CancellationTokenSource cancellationTokenSource = new();
                    cancellationTokenSource.CancelAfter(5000);
                    // 既存のウィンドウを表示
                    ShowAlreadyWindowAsync(cancellationTokenSource.Token).Wait();
                }
            }
            finally
            {
                if (createdNew) mutex.ReleaseMutex();
            }
        }
    }

    private static async Task ShowAlreadyWindowAsync(CancellationToken cancellationToken)
    {
        // 接続するだけで Show 依頼ということにする
        using var client = new NamedPipeClientStream(".", App.NamedPipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        await client.ConnectAsync(cancellationToken);
    }
}
