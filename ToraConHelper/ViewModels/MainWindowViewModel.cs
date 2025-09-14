using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using ToraConHelper.Services.TelemetryActions;
using ToraConHelper.Views;
using Wpf.Ui.Controls;

namespace ToraConHelper.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    private ObservableCollection<INavigationViewItem>? mainNavigationViewItems;
    [ObservableProperty]
    private ObservableCollection<INavigationViewItem>? footerNavigationViewItems;

    private readonly ViewModel viewModel;

    public MainWindowViewModel(ViewModel viewModel)
    {
        this.viewModel = viewModel;

        viewModel.GameInfoAction.GameInfoUpdated += OnGameInfoUpdated;
        viewModel.GameProcessDetector.GameProcessEnded += OnGameProcessEnded;

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

    [ObservableProperty]
    private bool isRunning = false;

    private void OnGameInfoUpdated(object sender, GameInfoUpdatedEventArgs e) => IsRunning = true;

    private void OnGameProcessEnded(object sender, EventArgs e) => IsRunning = false;

    public void Dispose()
    {
        viewModel.GameInfoAction.GameInfoUpdated -= OnGameInfoUpdated;
        viewModel.GameProcessDetector.GameProcessEnded -= OnGameProcessEnded;
    }
}
