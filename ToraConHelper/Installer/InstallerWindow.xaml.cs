using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ToraConHelper.Installer;

/// <summary>
/// InstallerWindow.xaml の相互作用ロジック
/// </summary>
public partial class InstallerWindow
{
    private readonly InstallerWindowViewModel viewModel = new();
    public InstallerWindow()
    {
        InitializeComponent();

        DataContext = viewModel;

        Loaded += InstallerWindow_Loaded;
    }

    private void InstallerWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Wpf.Ui.Appearance.SystemThemeWatcher.Watch(
            this,                                    // Window class
            Wpf.Ui.Controls.WindowBackdropType.Mica, // Background type
            true                                     // Whether to change accents automatically
        );

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
            viewModel.Message += $"{e.Message}{Environment.NewLine}";
        }));
    }

    private void closeButton_Click(object sender, RoutedEventArgs e) => Close();

    private void pluginFolderButton_Click(object sender, RoutedEventArgs e)
        => Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetDirectoryName(PluginInstaller.SourcePath)));
}

public partial class InstallerWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string message = string.Empty;
}
