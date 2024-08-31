using System.Collections.ObjectModel;
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

    public ObservableCollection<int> SteeringRotationAngles { get; private set; } = 
        new (){ 
            360,
            540,
            720,
            900,
            1080,
            1260,
            1440,
            1620,
            1800,
        };
}
