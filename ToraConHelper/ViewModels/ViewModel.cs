using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SCSSdkClient.Object;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using ToraConHelper.Services;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.ViewModels;

public partial class ViewModel : ObservableObject, IDisposable
{
    private const string SettingAppStartupUrl = "ms-settings:startupapps";
    private readonly bool isInitialization;
    private readonly ISettingFileMamager settingFile;

    private GameTimeAction gameInTimeAction;

    internal GameProcessDetector GameProcessDetector { get; private set; }

    internal TelemetryActionsManager TelemetryActionsManager { get; private set; }

    public ViewModel(ISettingFileMamager settingFile, TelemetryActionsManager telemetryActionsManager, GameProcessDetector gameProcessDetector) : base()
    {
        this.settingFile = settingFile;
        TelemetryActionsManager = telemetryActionsManager;
        GameProcessDetector = gameProcessDetector;

        gameProcessDetector.GameProcessEnded += GameProcessDetector_GameProcessEnded;
        if (!gameProcessDetector.IsStarted) gameProcessDetector.StartWatchers();

        isInitialization = true;
        LoadFromSettings(this.settingFile);
        isInitialization = false;

        gameInTimeAction = App.Current.Services.GetService<GameTimeAction>()!;
        gameInTimeAction.GameTimeUpdated += GameInTimeAction_GameTimeUpdated;
        TelemetryActionsManager.AddAction(gameInTimeAction);
    }

    private void GameProcessDetector_GameProcessEnded(object sender, EventArgs e)
    {
        // Game process has ended, update the game time
        GameTime = null;
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


    // Powertoys Page の時刻表示用
    private void GameInTimeAction_GameTimeUpdated(SCSTelemetry.Time gameTime)
        => GameTime = gameTime.Date;

    [ObservableProperty]
    private DateTime? gameTime;

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
        gameInTimeAction.GameTimeUpdated -= GameInTimeAction_GameTimeUpdated;

    }
}
