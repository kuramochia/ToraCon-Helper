using Microsoft.Extensions.DependencyInjection;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using ToraConHelper.Services;
using ToraConHelper.ViewModels;
using ToraConHelper.Views;

namespace ToraConHelper;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ViewModel viewModel;

    public MainWindow(ViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        // 最初は HomePage
        navigationView.SelectedItem = navigationView.MenuItems[0];

        if (!viewModel.TaskTrayOnStart)
        {
            this.Show();
        }

        var g = App.Current.Services.GetService<GameProcessDetector>();
        g?.StartWatchers();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (viewModel.GoToTasktrayOnAppClose)
        {
            e.Cancel = true;
            Hide();
            ShowInTaskbar = false;
        }
        else
        {
            base.OnClosing(e);
        }
    }

    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        var selectedItem = args.SelectedItem as NavigationViewItem;
        if (selectedItem == null) return;

        var enumName = selectedItem?.Tag.ToString();
        NavigationPageId pageId = (NavigationPageId)Enum.Parse(typeof(NavigationPageId), enumName);
        sender.Header = enumName;
        contentFrame.Navigate(App.Current.Services.GetService(NaviPages.PageIndex[pageId]));
    }
}

internal enum NavigationPageId
{
    Home,
    About
}


internal static class NaviPages
{
    public static readonly IReadOnlyDictionary<NavigationPageId, Type> PageIndex =
        new Dictionary<NavigationPageId, Type>()
        {
            { NavigationPageId.Home, typeof(HomePage) },
            { NavigationPageId.About, typeof(AboutPage) }
        };
}
