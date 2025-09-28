using System;
using System.ComponentModel;
using System.Windows.Threading;
using ToraConHelper.ViewModels;
using ToraConHelper.Views;
using Wpf.Ui.Abstractions;

namespace ToraConHelper;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly ViewModel viewModel;

    private WindowPositionSettings windowPositionSettings;

    private DispatcherTimer stateSaveTimer = new() { Interval = TimeSpan.FromSeconds(1) };

    public MainWindow(ViewModel viewModel, MainWindowViewModel mainWindowViewModel, INavigationViewPageProvider navigationViewPageProvider)
    {
        InitializeComponent();
        windowPositionSettings = new(this);
        this.viewModel = viewModel;
        DataContext = mainWindowViewModel;

        navigationView.SetPageProviderService(navigationViewPageProvider);

        stateSaveTimer.Tick += async (sender, args) =>
        {
            stateSaveTimer.Stop();
            if (IsLoaded && WindowState != System.Windows.WindowState.Minimized)
            {
                Width = ActualWidth;
                Height = ActualHeight;
                UpdateLayout();
                await windowPositionSettings.SaveWindowStateAsync();
            }
        };

        SourceInitialized += (sender, args) => windowPositionSettings.LoadWindowState();
        SizeChanged += (sender, args) => { if (!stateSaveTimer.IsEnabled) stateSaveTimer.Start(); };
        LocationChanged += (sender, args) => { if (!stateSaveTimer.IsEnabled) stateSaveTimer.Start(); };

        Loaded += (sender, args) =>
        {
            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(
                this,                                    // Window class
                Wpf.Ui.Controls.WindowBackdropType.Mica, // Background type
                true                                     // Whether to change accents automatically
            );

            // 最後に表示した Page を保存
            navigationView.Navigated += (sender, args) => viewModel.LastShownPage = args.Page.GetType().Name;

            viewModel.LastShownPage ??= "HomePage";
            navigationView.Navigate(viewModel.LastShownPage);
        };

        if (!viewModel.TaskTrayOnStart)
        {
            this.Show();
            WindowState = viewModel.MinimizeOnStart ? System.Windows.WindowState.Minimized : System.Windows.WindowState.Normal;
        }

        mainWindowViewModel.PropertyChanged += async (sender, args) =>
        {
            if (args.PropertyName == nameof(MainWindowViewModel.IsRunning))
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    // Taskbar の オーバーレイアイコン表示切り替え
                    taskbarItemInfo.Overlay = mainWindowViewModel.IsRunning ? (System.Windows.Media.ImageSource)FindResource("RunningOverlayIcon") : null;
                });
            }
        };
    }

    public void ShowHomePage() => navigationView.Navigate("HomePage");
    public void ShowPowerToysPage() => navigationView.Navigate("PowerToysPage");

    protected override void OnClosing(CancelEventArgs e)
    {
        if (viewModel.GoToTasktrayOnAppClose)
        {
            e.Cancel = true;
            Hide();
            ShowInTaskbar = false;
        }
        else
        {
            base.OnClosing(e);
        }
    }
}
