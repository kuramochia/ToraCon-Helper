using System.Windows.Controls;
using ToraConHelper.ViewModels;

namespace ToraConHelper.Views;

/// <summary>
/// PowerToysPage.xaml の相互作用ロジック
/// </summary>
public partial class PowerToysPage : Page
{
    public PowerToysPage(ViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
