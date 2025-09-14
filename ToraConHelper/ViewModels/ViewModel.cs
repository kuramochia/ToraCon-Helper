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
    private readonly ISettingFileMamager settingFile;

    internal GameInfoAction GameInfoAction { get; private set; }

    internal GameProcessDetector GameProcessDetector { get; private set; }

    internal TelemetryActionsManager TelemetryActionsManager { get; private set; }

    public ViewModel(ISettingFileMamager settingFile, TelemetryActionsManager telemetryActionsManager, GameProcessDetector gameProcessDetector, GameInfoAction gameInfoAction) : base()
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

    private void GameProcessDetector_GameProcessEnded(object sender, EventArgs e)
    {
        // Game process has ended, reset some properties
        GameTime = null;
        GameName = null;
        NavigationDistance = null;
        NavigationTime = null;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (IsSaveTargetName(e.PropertyName)) Save();
    }

    private static readonly HashSet<string> IgnoreSaveTargetNames = new(new[]
    {
        nameof(GameTime),
        nameof(GameName),
        nameof(NavigationDistance),
        nameof(NavigationTime),
    }, StringComparer.InvariantCultureIgnoreCase);

    private bool IsSaveTargetName(string propertyName) => !IgnoreSaveTargetNames.Contains(propertyName);

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


    #region Powertoys Page の情報表示用

    // Powertoys Page の情報表示用
    private void GameInfoAction_GameInfoUpdated(object sender, GameInfoUpdatedEventArgs e)
    {
        var currentGameTime = e.Telemetry.CommonValues.GameTime.Date;
        if (GameTime != currentGameTime) GameTime = currentGameTime;

        var currentGameName = e.Telemetry.Game.ToString().ToUpper();
        if (GameName != currentGameName) GameName = currentGameName;

        var currentNavigationDistance = e.Telemetry.NavigationValues.NavigationDistance / 1000; // m to km
        if (NavigationDistance != currentNavigationDistance) NavigationDistance = currentNavigationDistance;

        var currentNavigationTime = TimeSpan.FromSeconds(e.Telemetry.NavigationValues.NavigationTime);
        if (NavigationTime != currentNavigationTime) NavigationTime = currentNavigationTime;
    }

    // ゲーム内時間
    [ObservableProperty]
    private DateTime? gameTime;

    // ゲーム名
    [ObservableProperty]
    private string? gameName;

    [ObservableProperty]
    private float? navigationDistance;

    [ObservableProperty]
    private TimeSpan? navigationTime;

    #endregion

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
