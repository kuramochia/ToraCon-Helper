using System.ComponentModel;
using ToraConHelper.ViewModels;
using Wpf.Ui.Abstractions;

namespace ToraConHelper;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly ViewModel viewModel;

    public MainWindow(ViewModel viewModel, MainWindowViewModel mainWindowViewModel, INavigationViewPageProvider navigationViewPageProvider)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        DataContext = mainWindowViewModel;

        navigationView.SetPageProviderService(navigationViewPageProvider);

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
            WindowState = viewModel.MinimizeOnStart ? System.Windows.WindowState.Minimized : System.Windows.WindowState.Normal;
            this.Show();
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
