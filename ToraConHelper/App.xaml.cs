using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using ToraConHelper.Services;
using ToraConHelper.Services.TelemetryActions;
using ToraConHelper.ViewModels;
using ToraConHelper.Views;

namespace ToraConHelper;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App() : base()
    {
        Services = ConfigureServices();
        InitializeComponent();
    }

    private System.Windows.Forms.NotifyIcon? notifyIcon;

    private EventHandler? showAction;
    public new static App Current => (App)Application.Current;

    public IServiceProvider Services { get; }


    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // ちょっと雑だけど、MainWindowを DI から取る -> ViewModel が DI から取る → 初期化と動作開始、という流れ
        MainWindow = Services.GetRequiredService<MainWindow>();
        //MainWindow.Show();

        showAction = (object s, EventArgs e) =>
        {
            MainWindow.ShowInTaskbar = true;
            MainWindow.Show();
            MainWindow.Activate();
        };

        // タスクトレイアイコン
        var icon = GetResourceStream(new Uri("icon.ico", UriKind.Relative)).Stream;
        var menu = new System.Windows.Forms.ContextMenuStrip();
        menu.Items.Add("表示", null, showAction);
        menu.Items.Add("終了", null, (s, e) => Shutdown());
        notifyIcon = new System.Windows.Forms.NotifyIcon
        {
            Visible = true,
            Icon = new System.Drawing.Icon(icon),
            Text = "ToraCon Helper",
            ContextMenuStrip = menu,
        };
        notifyIcon.DoubleClick += showAction;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Singleton Cleanup
        (Services as IDisposable)?.Dispose();
        notifyIcon!.DoubleClick -= showAction;
        notifyIcon!.Dispose();
        base.OnExit(e);
    }

    private ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        // Views
        services.AddSingleton<MainWindow>();
        services.AddSingleton<HomePage>();
        services.AddSingleton<AboutPage>();

        // ViewModels
        services.AddSingleton<ViewModel>();

        // Services
        services.AddSingleton<ISettingFileMamager, SettingFileManager>();
        services.AddSingleton<GameProcessDetector>();
        services.AddSingleton<TelemetryActionsManager>();

        // Services.TelemetryActions
        services.AddSingleton<BlinkerLikeRealCarAction>();
        services.AddSingleton<ReterderAllReduceAction>();

        return services.BuildServiceProvider();
    }
}
