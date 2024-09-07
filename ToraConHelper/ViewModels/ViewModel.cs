using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using ToraConHelper.Services;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.ViewModels;

public partial class ViewModel : ObservableObject
{
    private readonly bool _isInitialization;
    private readonly ISettingFileMamager settingFile;
    private readonly TelemetryActionsManager telemetryActionsManager;

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
        var s = new Settings
        {
            IsActive = IsActive,
            GoToTasktrayOnAppClose = GoToTasktrayOnAppClose,
            TaskTrayOnStart = TaskTrayOnStart,

            BlinkerLikeRealCarActionEnabled = BlinkerLikeRealCarActionEnabled,
            RetarderAllReduceActionEnabled = ReterderAllReduceActionEnabled,
            RetarderAllReduceActionLimitSpeedKph = ReterderAllReduceActionLimitSpeedKph,
            BlinkerHideOnSteeringActionEnabled = BlinkerHideOnSteeringActionEnabled,
            SteeringRotationAngle = SteeringRotationAngle,
            BlinkerHideBySteeringAngle = BlinkerHideBySteeringAngle,
            RetarderFullOnActionEnabled = RetarderFullOnActionEnabled,
            RetarderFullOffActionEnabled = RetarderFullOffActionEnabled
        };
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
        BlinkerHideOnSteeringActionEnabled = s.BlinkerHideOnSteeringActionEnabled;
        SteeringRotationAngle = s.SteeringRotationAngle;
        BlinkerHideBySteeringAngle = s.BlinkerHideBySteeringAngle;
        RetarderFullOnActionEnabled = s.RetarderFullOnActionEnabled;
        RetarderFullOffActionEnabled = s.RetarderFullOffActionEnabled;
    }

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
    private bool blinkerLikeRealCarActionEnabled;

    partial void OnBlinkerLikeRealCarActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<BlinkerLikeRealCarAction>(oldValue, newValue);

    [ObservableProperty]
    private bool reterderAllReduceActionEnabled;

    partial void OnReterderAllReduceActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<ReterderAllReduceAction>(oldValue, newValue);

    [ObservableProperty]
    private int reterderAllReduceActionLimitSpeedKph;

    partial void OnReterderAllReduceActionLimitSpeedKphChanged(int newValue)
    {
        var action = App.Current.Services.GetService<ReterderAllReduceAction>();
        action!.LimitSpeedKph = newValue;
    }

    [ObservableProperty]
    private bool blinkerHideOnSteeringActionEnabled;

    partial void OnBlinkerHideOnSteeringActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<BlinkerHideOnSteeringAction>(oldValue, newValue);

    // ハンドル回転角
    [ObservableProperty]
    private int steeringRotationAngle;

    partial void OnSteeringRotationAngleChanged(int value) => SteeringAngleChanged();
    // ウィンカー消す角度
    [ObservableProperty]
    private int blinkerHideBySteeringAngle;

    partial void OnBlinkerHideBySteeringAngleChanged(int newValue) => SteeringAngleChanged();

    private void SteeringAngleChanged()
    {
        var action = App.Current.Services.GetService<BlinkerHideOnSteeringAction>();
        // 角度から相対値に変換
        action!.BlinkerHidePosition = (float)BlinkerHideBySteeringAngle / (SteeringRotationAngle / 2);
    }

    // リターダーを一段入れたら最大段数まで上げる
    [ObservableProperty]
    private bool retarderFullOnActionEnabled;

    partial void OnRetarderFullOnActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<RetarderFullOnAction>(oldValue, newValue);


    // リターダーをフルから一段下げたら 0 段にする
    [ObservableProperty]
    private bool retarderFullOffActionEnabled;

    partial void OnRetarderFullOffActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<RetarderFullOffAction>(oldValue, newValue);

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
