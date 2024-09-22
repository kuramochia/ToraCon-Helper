using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Windows;
using ToraConHelper.Installer;
using ToraConHelper.Services;
using ToraConHelper.Services.TelemetryActions;
using ToraConHelper.ViewModels;
using ToraConHelper.Views;
using Wpf.Ui;

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
        // ちょっと雑だけど、MainWindowを DI から取る -> ViewModel を DI から取る → 初期化と動作開始、という流れ
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

        // Telemetry DLL 更新チェック
        CheckTelemetryDLL();
    }

    private void CheckTelemetryDLL()
    {
        var installer = new PluginInstaller();
        if (installer.NeedInstall())
        {
            var msg = $"Telemetry DLL の更新が必要です。更新しますか？{Environment.NewLine}(※)管理者権限が必要です";
            if (MessageBox.Show(msg, MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Create new process
                var pInfo = new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().Location, "-install");
                pInfo.Verb = "runas";
                Process.Start(pInfo);
            }
        }
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
        services.AddSingleton<IPageService,PageService>();

        // ViewModels
        services.AddSingleton<ViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        // Services
        services.AddSingleton<ISettingFileMamager, SettingFileManager>();
        services.AddSingleton<TelemetryActionsManager>();

        // Services.TelemetryActions
        services.AddSingleton<BlinkerLikeRealCarAction>();
        services.AddSingleton<ReterderAllReduceAction>();
        services.AddSingleton<BlinkerHideOnSteeringAction>();
        services.AddSingleton<RetarderFullOnAction>();
        services.AddSingleton<RetarderFullOffAction>();
        services.AddSingleton<EngineBrakeAutoOffAction>();
        services.AddSingleton<ReterderAutoOffAction>();
        services.AddSingleton<AutoFullfuelAction>();

        return services.BuildServiceProvider();
    }
}
