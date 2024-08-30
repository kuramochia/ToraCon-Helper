using System.Windows.Controls;
using ToraConHelper.ViewModels;

namespace ToraConHelper.Views;

/// <summary>
/// HomePage.xaml の相互作用ロジック
/// </summary>
public partial class HomePage : Page
{
    public HomePage(ViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
