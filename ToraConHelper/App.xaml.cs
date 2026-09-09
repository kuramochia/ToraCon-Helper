using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ToraConHelper.Installer;
using ToraConHelper.Services;
using ToraConHelper.Services.TelemetryActions;
using ToraConHelper.ViewModels;
using ToraConHelper.Views;
using Wpf.Ui.Abstractions;
using WpfUi = Wpf.Ui.Controls;

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

    internal EventWaitHandle SingleInstanceEvent { private get; set; } = null!;

    private RegisteredWaitHandle? registeredWaitHandle;

    private bool isExiting;

    private System.Windows.Forms.NotifyIcon? notifyIcon;

    private EventHandler? showAction;

    private EventHandler? showPowerToysAction;

    public new static App Current => (App)Application.Current;

    public IServiceProvider Services { get; }

    private MessageOnlyWindow? messageOnlyWindow;


    protected override async void OnStartup(StartupEventArgs e)
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

            // 最小化していたら表示する
            if (MainWindow.WindowState == WindowState.Minimized) MainWindow.WindowState = WindowState.Normal;
        };

        showPowerToysAction = (object s, EventArgs e) =>
        {
            showAction(s, e);
            ((MainWindow)MainWindow).ShowPowerToysPage();
        };

        // タスクトレイアイコン
        using var icon = GetResourceStream(new Uri("icon.ico", UriKind.Relative)).Stream;
        var menu = new System.Windows.Forms.ContextMenuStrip();
        menu.Items.Add("表示", null, showAction);
        menu.Items.Add("PowerToys を表示", null, showPowerToysAction);
        menu.Items.Add("終了", null, (s, e) => Shutdown());
        notifyIcon = new System.Windows.Forms.NotifyIcon
        {
            Visible = true,
            Icon = new System.Drawing.Icon(icon),
            Text = "ToraCon Helper",
            ContextMenuStrip = menu,
        };
        notifyIcon.DoubleClick += showAction;

        // 多重起動時の表示依頼を拾う
        registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
            SingleInstanceEvent,
            OnSingleInstanceEventSignaled,
            null,
            Timeout.Infinite,
            false);

        // Telemetry DLL 更新チェック
        await CheckTelemetryDLLAsync();

        // メッセージ専用ウィンドウを作成
        messageOnlyWindow = new();
        messageOnlyWindow.Show();
    }

    private async Task CheckTelemetryDLLAsync()
    {
        var installer = new PluginInstaller();
        if (installer.NeedInstall())
        {
            var msg = $"Telemetry DLL が更新されています。インストールを行いますか？{Environment.NewLine}管理者権限が必要です。";
            var msgBox = new WpfUi.MessageBox
            {
                Title = MainWindow.Title,
                Content = msg,
                PrimaryButtonText = "はい",
                CloseButtonText = "いいえ",
                IsPrimaryButtonEnabled  = true,
                IsCloseButtonEnabled = true,
            };
            if((await msgBox.ShowDialogAsync()) == WpfUi.MessageBoxResult.Primary)
            {
                // Create new process
                var pInfo = new ProcessStartInfo("ToraConHelper_installer.exe", "-install")
                {
                    Verb = "runas",
                    UseShellExecute = true,
                };
                Process.Start(pInfo);
            }
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        isExiting = true;
        registeredWaitHandle?.Unregister(null);
        registeredWaitHandle = null;
        // Singleton Cleanup
        (Services as IDisposable)?.Dispose();
        notifyIcon!.DoubleClick -= showAction;
        notifyIcon!.Dispose();
        messageOnlyWindow?.Close();
        messageOnlyWindow?.Dispose();
        base.OnExit(e);
    }

    private ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        // Views
        services.AddSingleton<MainWindow>();
        services.AddSingleton<HomePage>();
        services.AddSingleton<AboutPage>();
        services.AddSingleton<PowerToysPage>();
        services.AddSingleton<INavigationViewPageProvider, NavigationViewPageService>();

        // ViewModels
        services.AddSingleton<ViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        // Services
        services.AddSingleton<ISettingFileManager, SettingFileManager>();
        services.AddSingleton<TelemetryActionsManager>();
        services.AddSingleton<GameProcessDetector>();

        // Services.TelemetryActions
        services.AddSingleton<BlinkerLikeRealCarAction>();
        services.AddSingleton<RetarderAllReduceAction>();
        services.AddSingleton<BlinkerHideOnSteeringAction>();
        services.AddSingleton<RetarderFullOnAction>();
        services.AddSingleton<RetarderFullOffAction>();
        services.AddSingleton<EngineBrakeAutoOffAction>();
        services.AddSingleton<RetarderAutoOffAction>();
        services.AddSingleton<AutoFullFuelAction>();
        services.AddSingleton<BlinkerForLaneChangeAction>();
        services.AddSingleton<BlinkerLikeRealCarDInputAction>();
        services.AddSingleton<RetarderSkipInputAction>();
        services.AddSingleton<RetarderAllReduceOnThrottleAction>();
        services.AddSingleton<GameInfoAction>();
        services.AddSingleton<FollowSpeedLimitCruiseControlAction>();
        services.AddSingleton<AutoFlasherAtReverseAction>();

        return services.BuildServiceProvider();
    }

    private void OnSingleInstanceEventSignaled(object? state, bool timedOut)
    {
        if (timedOut || isExiting)
        {
            return;
        }

        Dispatcher.BeginInvoke(new Action(() =>
        {
            if (!isExiting)
            {
                showAction?.Invoke(this, EventArgs.Empty);
            }
        }));
    }
}
