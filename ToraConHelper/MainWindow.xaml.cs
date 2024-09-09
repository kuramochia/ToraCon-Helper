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
            navigationView.Navigate("HomePage");
        };

        if (!viewModel.TaskTrayOnStart)
        {
            this.Show();
        }
    }

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
