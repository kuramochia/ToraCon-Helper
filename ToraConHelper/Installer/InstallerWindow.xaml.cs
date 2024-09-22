using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ToraConHelper.Installer;

/// <summary>
/// InstallerWindow.xaml の相互作用ロジック
/// </summary>
public partial class InstallerWindow : Window
{
    private readonly ObservableCollection<string> messages = new ();
    public InstallerWindow()
    {
        InitializeComponent();

        DataContext = messages;

        Loaded += InstallerWindow_Loaded;
    }

    private void InstallerWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _ = Task.Run(() =>
        {
            var pluginInstaller = new PluginInstaller();
            pluginInstaller.AddMessageFromInstaller += PluginInstaller_AddMessageFromInstaller;
            pluginInstaller.InstallProcess();
        });
    }

    private void PluginInstaller_AddMessageFromInstaller(object sender, AddMessageEventArgs e)
    {
        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
        {
            messages.Add(e.Message);
        }));
    }
}

public class PluginApp : Application
{
    public PluginApp() : base()
    {
        MainWindow = new InstallerWindow();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        MainWindow.Show();
    }
}
