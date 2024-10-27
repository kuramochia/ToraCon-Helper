using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using ToraConHelper.Services;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.ViewModels;

public partial class ViewModel : ObservableObject, IDisposable
{
    private readonly bool isInitialization;
    private readonly ISettingFileMamager settingFile;
    private readonly TelemetryActionsManager telemetryActionsManager;
    private readonly GameProcessDetector gameProcessDetector;
    private readonly ShortcutService shortcutService;
    internal GameProcessDetector GameProcessDetector { get { return gameProcessDetector; } }

    public ViewModel(ISettingFileMamager settingFile, TelemetryActionsManager telemetryActionsManager, GameProcessDetector gameProcessDetector, ShortcutService shortCutService) : base()
    {
        this.settingFile = settingFile;
        this.telemetryActionsManager = telemetryActionsManager;
        this.gameProcessDetector = gameProcessDetector;
        this.shortcutService = shortCutService;

        isInitialization = true;
        LoadFromSettings(this.settingFile);
        isInitialization = false;

        IsShortcutCreated = shortcutService.HasShortcutFile();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        Save();
    }

    internal void Save()
    {
        if (!isInitialization) settingFile.Save(ToSettings());
    }

    #region General Settings
    [ObservableProperty]
    private bool isActive;

    partial void OnIsActiveChanged(bool oldValue, bool newValue)
    {
        if (oldValue)
        {
            telemetryActionsManager?.Stop();
        }
        if (newValue)
        {
            telemetryActionsManager?.Start();
        }
    }

    [ObservableProperty]
    private bool goToTasktrayOnAppClose;

    [ObservableProperty]
    private bool taskTrayOnStart;

    [ObservableProperty]
    private string? lastShownPage;
    #endregion

    #region Shortcut

    [ObservableProperty]
    private bool isShortcutCreated;

    partial void OnIsShortcutCreatedChanged(bool value)
    {
        if (value)
        {
            shortcutService.Create();
        }
        else
        {
            shortcutService.Delete();
        }
        IsShortcutCreated = shortcutService.HasShortcutFile();
    }

    #endregion

    #region Private methods
    private void OnActionEnabledChanged<T>(bool oldValue, bool newValue) where T : ITelemetryAction
    {
        var action = App.Current.Services.GetService<T>();
        if (oldValue)
        {
            telemetryActionsManager?.RemoveAction(action!);
        }
        if (newValue)
        {
            telemetryActionsManager?.AddAction(action!);
        }
    }
    #endregion
    public void Dispose()
    {
        Ets2?.Dispose();
        Ats?.Dispose();
    }
}
