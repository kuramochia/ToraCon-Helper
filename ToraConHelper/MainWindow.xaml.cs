using System.ComponentModel;
using ToraConHelper.ViewModels;
using Wpf.Ui;

namespace ToraConHelper;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly ViewModel viewModel;

    public MainWindow(ViewModel viewModel, MainWindowViewModel mainWindowViewModel, IPageService pageService)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        DataContext = mainWindowViewModel;

        navigationView.SetPageService(pageService);

        Loaded += (sender, args) =>
        {
            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(
                this,                                    // Window class
                Wpf.Ui.Controls.WindowBackdropType.Mica, // Background type
                true                                     // Whether to change accents automatically
            );

            // 最初は HomePage
            ShowHomePage();
        };

        if (!viewModel.TaskTrayOnStart)
        {
            this.Show();
        }
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
