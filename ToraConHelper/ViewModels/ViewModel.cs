using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using ToraConHelper.Services;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.ViewModels;

public partial class ViewModel : ObservableObject
{
    private readonly bool isInitialization;
    private readonly ISettingFileMamager settingFile;
    private readonly TelemetryActionsManager telemetryActionsManager;

    public ViewModel(ISettingFileMamager settingFile, TelemetryActionsManager telemetryActionsManager) : base()
    {
        this.settingFile = settingFile;
        this.telemetryActionsManager = telemetryActionsManager;

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
}
