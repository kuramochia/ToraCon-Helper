using System;
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
           _ = Dispatcher.InvokeAsync(() => realTimeText.Text = DateTime.Now.ToString("HH:mm:ss"));
        };
        timer.Start();
    }
}
