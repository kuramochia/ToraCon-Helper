using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
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
    internal const string NamedPipeName = "ToraConHelper-NamedPipe-4-MultiInstance";

    public App() : base()
    {
        Services = ConfigureServices();
        InitializeComponent();
    }

    private System.Windows.Forms.NotifyIcon? notifyIcon;

    private EventHandler? showAction;

    private EventHandler? showPowerToysAction;

    private CancellationTokenSource cancellationTokenSource = new();

    public new static App Current => (App)Application.Current;

    public IServiceProvider Services { get; }


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
        _ = RunPerProcessCommunicationAsync(cancellationTokenSource.Token).ConfigureAwait(false);

        // Telemetry DLL 更新チェック
        CheckTelemetryDLL();
    }

    private void CheckTelemetryDLL()
    {
        var installer = new PluginInstaller();
        if (installer.NeedInstall())
        {
            var msg = $"Telemetry DLL が更新されています。インストールを行いますか？{Environment.NewLine}管理者権限が必要です。";
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
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
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
        services.AddSingleton<PowerToysPage>();
        services.AddSingleton<IPageService, PageService>();

        // ViewModels
        services.AddSingleton<ViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        // Services
        services.AddSingleton<ISettingFileMamager, SettingFileManager>();
        services.AddSingleton<TelemetryActionsManager>();
        services.AddSingleton<GameProcessDetector>();

        // Services.TelemetryActions
        services.AddSingleton<BlinkerLikeRealCarAction>();
        services.AddSingleton<ReterderAllReduceAction>();
        services.AddSingleton<BlinkerHideOnSteeringAction>();
        services.AddSingleton<RetarderFullOnAction>();
        services.AddSingleton<RetarderFullOffAction>();
        services.AddSingleton<EngineBrakeAutoOffAction>();
        services.AddSingleton<ReterderAutoOffAction>();
        services.AddSingleton<AutoFullfuelAction>();
        services.AddSingleton<BlinkerForLaneChangeAction>();
        services.AddSingleton<BlinkerLikeRealCarDInputAction>();

        return services.BuildServiceProvider();
    }

    private async Task RunPerProcessCommunicationAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var server = new NamedPipeServerStream(NamedPipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                await server.WaitForConnectionAsync(cancellationToken);
                // 特別な通信内容はいらないので、接続されたら Show() の依頼、ということにする
                await Dispatcher.InvokeAsync(() => showAction?.Invoke(this, EventArgs.Empty));
            }
            catch
            {
                continue;
            }
        }
    }
}
