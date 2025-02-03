using System;
using System.Windows;
using Wpf.Ui.Abstractions;

namespace ToraConHelper.Views;

public class NavigationViewPageService(IServiceProvider serviceProvider) : INavigationViewPageProvider
{
    public object? GetPage(Type pageType)
    {
        if (!typeof(FrameworkElement).IsAssignableFrom(pageType))
        {
            throw new InvalidOperationException("The page should be a WPF control.");
        }

        return serviceProvider.GetService(pageType);
    }
}
