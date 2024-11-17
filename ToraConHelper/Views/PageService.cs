using System;
using System.Windows;
using Wpf.Ui;

namespace ToraConHelper.Views;

public class PageService(IServiceProvider serviceProvider) : IPageService
{
    public T? GetPage<T>()
        where T : class
    {
        if (!typeof(FrameworkElement).IsAssignableFrom(typeof(T)))
        {
            throw new InvalidOperationException("The page should be a WPF control.");
        }

        return (T?)serviceProvider.GetService(typeof(T));
    }

    public FrameworkElement? GetPage(Type pageType)
    {
        if (!typeof(FrameworkElement).IsAssignableFrom(pageType))
        {
            throw new InvalidOperationException("The page should be a WPF control.");
        }

        return serviceProvider.GetService(pageType) as FrameworkElement;
    }
}
