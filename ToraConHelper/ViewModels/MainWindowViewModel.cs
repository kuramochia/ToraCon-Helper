using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using ToraConHelper.Views;
using Wpf.Ui.Controls;

namespace ToraConHelper.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<INavigationViewItem>? mainNavigationViewItems;
    [ObservableProperty]
    private ObservableCollection<INavigationViewItem>? footerNavigationViewItems;

    public MainWindowViewModel()
    {
        MainNavigationViewItems =
            [
                new NavigationViewItem{
                    Content="Home",
                    Icon = new SymbolIcon{ Symbol = SymbolRegular.Home24 },
                    TargetPageType = typeof(HomePage),
                    TargetPageTag = nameof(HomePage),
                },
                new NavigationViewItem {
                    Content="PowerToys",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Toolbox24 },
                    TargetPageType = typeof(PowerToysPage),
                    TargetPageTag= nameof(PowerToysPage),
                },
            ];

        FooterNavigationViewItems =
            [
                new NavigationViewItem{
                    Content = "About",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.PersonQuestionMark24 },
                    TargetPageType= typeof(AboutPage),
                    TargetPageTag= nameof(AboutPage),
                },
            ];
    }
}
