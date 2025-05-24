using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
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
    private readonly TelemetryActionsManager telemetryActionsManager;
    private readonly GameProcessDetector gameProcessDetector;
    internal GameProcessDetector GameProcessDetector { get { return gameProcessDetector; } }

    public ViewModel(ISettingFileMamager settingFile, TelemetryActionsManager telemetryActionsManager, GameProcessDetector gameProcessDetector) : base()
    {
        this.settingFile = settingFile;
        this.telemetryActionsManager = telemetryActionsManager;
        this.gameProcessDetector = gameProcessDetector;

        isInitialization = true;
        LoadFromSettings(this.settingFile);
        isInitialization = false;

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
