using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace ToraConHelper.Views;

public class WindowPositionSettings
{

    private Window _window;

    public WindowPositionSettings(Window window) => _window = window;

    private bool _isLoading = false;
    public void LoadWindowState()
    {
        var settings = Properties.Settings.Default;
        settings.Reload();

        if (settings.WindowHeight == 0d) return;
        try
        {
            _isLoading = true;
            _window.Left = settings.WindowLeft;
            _window.Top = settings.WindowTop;
            _window.Width = settings.WindowWidth;
            _window.Height = settings.WindowHeight;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load window state:\r\n{ex}");
        }
        finally
        {
            _isLoading = false;
        }
    }

    public Task SaveWindowStateAsync()
    {
        if (_isLoading) return Task.CompletedTask;

        var settings = Properties.Settings.Default;
        settings.WindowLeft = _window.Left;
        settings.WindowTop = _window.Top;
        settings.WindowWidth = _window.Width;
        settings.WindowHeight = _window.Height;
        return Task.Run(() => settings.Save());
    }
}
