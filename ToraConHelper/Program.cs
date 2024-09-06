using System;
using System.Threading;

namespace ToraConHelper;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
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
