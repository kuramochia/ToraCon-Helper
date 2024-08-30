using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using ToraConHelper.Services;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.ViewModels;

public partial class ViewModel : ObservableObject
{
    private bool _isInitialization;
    private ISettingFileMamager settingFile;
    private TelemetryActionsManager telemetryActionsManager;

    public ViewModel(ISettingFileMamager settingFile, TelemetryActionsManager telemetryActionsManager) : base()
    {
        this.settingFile = settingFile;
        this.telemetryActionsManager = telemetryActionsManager;

        _isInitialization = true;
        FromSettings(this.settingFile.Load());
        _isInitialization = false;
    }


    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (!_isInitialization)
        {
            settingFile.Save(ToSettings());
        }
    }

    private Settings ToSettings()
    {
        var s = new Settings();
        s.IsActive = IsActive;
        s.GoToTasktrayOnAppClose = GoToTasktrayOnAppClose;
        s.TaskTrayOnStart = TaskTrayOnStart;

        s.BlinkerLikeRealCarActionEnabled = BlinkerLikeRealCarActionEnabled;
        s.RetarderAllReduceActionEnabled = ReterderAllReduceActionEnabled;
        s.RetarderAllReduceActionLimitSpeedKph = ReterderAllReduceActionLimitSpeedKph;

        return s;
    }

    private void FromSettings(Settings s)
    {
        IsActive = s.IsActive;
        GoToTasktrayOnAppClose = s.GoToTasktrayOnAppClose;
        TaskTrayOnStart = s.TaskTrayOnStart;

        BlinkerLikeRealCarActionEnabled = s.BlinkerLikeRealCarActionEnabled;
        ReterderAllReduceActionEnabled = s.RetarderAllReduceActionEnabled;
        ReterderAllReduceActionLimitSpeedKph = s.RetarderAllReduceActionLimitSpeedKph;
    }

    [ObservableProperty]
    public bool isActive;

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
    public bool goToTasktrayOnAppClose;

    [ObservableProperty]
    public bool taskTrayOnStart;

    [ObservableProperty]
    public bool blinkerLikeRealCarActionEnabled;

    partial void OnBlinkerLikeRealCarActionEnabledChanged(bool oldValue, bool newValue)
    {
        OnActionEnabledChanged(typeof(BlinkerLikeRealCarAction), oldValue, newValue);
    }

    [ObservableProperty]
    public bool reterderAllReduceActionEnabled;

    partial void OnReterderAllReduceActionEnabledChanged(bool oldValue, bool newValue)
    {
        OnActionEnabledChanged(typeof(ReterderAllReduceAction), oldValue, newValue);
    }

    [ObservableProperty]
    public int reterderAllReduceActionLimitSpeedKph;

    partial void OnReterderAllReduceActionLimitSpeedKphChanged(int oldValue, int newValue)
    {
        var action = App.Current.Services.GetService<ReterderAllReduceAction>();
        action!.LimitSpeedKph = newValue;
    }

    #region Private methods

    private void OnActionEnabledChanged(Type actionType, bool oldValue, bool newValue)
    {
        var action = App.Current.Services.GetService(actionType) as ITelemetryAction;
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
