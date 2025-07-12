using System;
using System.Windows;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Threading;
using ToraConHelper.ViewModels;

namespace ToraConHelper.Views;

/// <summary>
/// PowerToysPage.xaml の相互作用ロジック
/// </summary>
public partial class PowerToysPage : Page
{
    private Timer timer = new(1000);
    public PowerToysPage(ViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        timer.Elapsed += (sender, e) =>
        {
            _ = Dispatcher.InvokeAsync(() =>
            {
                var nowStr = DateTime.Now.ToString("HH:mm:ss");
                realTimeText.Text = nowStr;
                realTimeTitleText.Text = nowStr;
            });
        };
        timer.Start();
    }

    private void gameInfoExpander_ExpandedChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (gameInfoExpander.IsExpanded)
        {
            Dispatcher.InvokeAsync(() => realTimeTitleText.Visibility = Visibility.Collapsed);
        }
        else
        {
            Dispatcher.InvokeAsync(() => realTimeTitleText.Visibility = Visibility.Visible);
        }
    }

}
