using System.Windows;

namespace ToraConHelper.Installer;

public partial class PluginApp : Application
{
    public PluginApp() : base()
    {
        InitializeComponent();
        MainWindow = new InstallerWindow();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        MainWindow?.Show();
    }
}
