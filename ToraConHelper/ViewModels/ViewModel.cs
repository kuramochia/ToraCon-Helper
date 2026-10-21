using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using ToraConHelper.Services;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.ViewModels;

public partial class ViewModel : ObservableObject, IDisposable
{
    private const string SettingAppStartupUrl = "ms-settings:startupapps";
    private readonly bool isInitialization;
    private readonly ISettingFileManager settingFile;

    internal GameInfoAction GameInfoAction { get; private set; }

    internal GameProcessDetector GameProcessDetector { get; private set; }

    internal TelemetryActionsManager TelemetryActionsManager { get; private set; }

    public ViewModel(ISettingFileManager settingFile, TelemetryActionsManager telemetryActionsManager, GameProcessDetector gameProcessDetector, GameInfoAction gameInfoAction) : base()
    {
        this.settingFile = settingFile;
        TelemetryActionsManager = telemetryActionsManager;
        GameProcessDetector = gameProcessDetector;

        gameProcessDetector.GameProcessEnded += GameProcessDetector_GameProcessEnded;
        if (!gameProcessDetector.IsStarted) gameProcessDetector.StartWatchers();

        isInitialization = true;
        LoadFromSettings(this.settingFile);
        isInitialization = false;

        GameInfoAction = gameInfoAction;
        gameInfoAction.GameInfoUpdated += GameInfoAction_GameInfoUpdated;
        TelemetryActionsManager.AddAction(gameInfoAction);
    }
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (IsSaveTargetName(e.PropertyName)) Save();
    }

    #region General Settings
    [ObservableProperty]
    private bool isActive;

    partial void OnIsActiveChanged(bool oldValue, bool newValue)
    {
        if (oldValue)
        {
            TelemetryActionsManager?.Stop();
        }
        if (newValue)
        {
            TelemetryActionsManager?.Start();
        }
    }

    [ObservableProperty]
    private bool goToTasktrayOnAppClose;

    [ObservableProperty]
    private bool taskTrayOnStart;

    [ObservableProperty]
    private bool minimizeOnStart;

    [RelayCommand]
    private void RegisterStartup() => Process.Start(SettingAppStartupUrl);

    [ObservableProperty]
    private string? lastShownPage;


    #endregion

    #region Private methods
    private void OnActionEnabledChanged<T>(bool oldValue, bool newValue) where T : ITelemetryAction
    {
        var action = App.Current.Services.GetService<T>();
        if (oldValue)
        {
            TelemetryActionsManager?.RemoveAction(action!);
        }
        if (newValue)
        {
            TelemetryActionsManager?.AddAction(action!);
        }
    }
    #endregion
    public void Dispose()
    {
        Ets2?.Dispose();
        Ats?.Dispose();
        GameProcessDetector.GameProcessEnded -= GameProcessDetector_GameProcessEnded;
        GameInfoAction.GameInfoUpdated -= GameInfoAction_GameInfoUpdated;

    }
}
