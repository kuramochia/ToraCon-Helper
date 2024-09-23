using CommunityToolkit.Mvvm.ComponentModel;
using Gameloop.Vdf.Linq;
using Microsoft.Extensions.DependencyInjection;
using System;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.ViewModels;

public partial class ViewModel
{
    [ObservableProperty]
    private bool blinkerLikeRealCarActionEnabled;

    partial void OnBlinkerLikeRealCarActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<BlinkerLikeRealCarAction>(oldValue, newValue);

    [ObservableProperty]
    private bool blinkerHideOnSteeringActionEnabled;

    partial void OnBlinkerHideOnSteeringActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<BlinkerHideOnSteeringAction>(oldValue, newValue);

    // ハンドル回転角
    [ObservableProperty]
    private int steeringRotationAngle;

    partial void OnSteeringRotationAngleChanged(int value)
    {
        BlinkerHideBySteeringAngleChanged();
        BlinkerForLaneChangeSteeringAngleChanged();
    }

    // ウィンカー消す角度
    [ObservableProperty]
    private int blinkerHideBySteeringAngle;

    partial void OnBlinkerHideBySteeringAngleChanged(int newValue) => BlinkerHideBySteeringAngleChanged();

    private void BlinkerHideBySteeringAngleChanged()
    {
        var action = App.Current.Services.GetService<BlinkerHideOnSteeringAction>();
        // 角度から相対値に変換
        action!.BlinkerHidePosition = (float)BlinkerHideBySteeringAngle / (SteeringRotationAngle / 2);
    }

    // 車線変更ウィンカーアクション
    [ObservableProperty]
    private bool blinkerForLaneChangeEnabled;

    partial void OnBlinkerForLaneChangeEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<BlinkerForLaneChangeAction>(oldValue, newValue);

    // 車線変更ウィンカー監視開始速度
    [ObservableProperty]
    private int blinkerForLaneChangeLimitSpeedKph;

    partial void OnBlinkerForLaneChangeLimitSpeedKphChanged(int value)
    {
        var action = App.Current.Services.GetService<BlinkerForLaneChangeAction>();
        action!.LowerLimitKph = value;
    }

    // 車線変更ウィンカー監視範囲ステアリング角度
    [ObservableProperty]
    private int blinkerForLaneChangeSteeringAngle;

    partial void OnBlinkerForLaneChangeSteeringAngleChanged(int value)
    {
        BlinkerForLaneChangeSteeringAngleChanged();
    }

    private void BlinkerForLaneChangeSteeringAngleChanged()
    {
        var action = App.Current.Services.GetService<BlinkerForLaneChangeAction>();
        // 角度から相対値に変換
        action!.SteeringLimit = (float)BlinkerForLaneChangeSteeringAngle / (SteeringRotationAngle / 2);
    }

    // 車線変更ウィンカーオフまでの時間(秒)
    [ObservableProperty]
    private int blinkerForLaneChangeOffSeconds;

    partial void OnBlinkerForLaneChangeOffSecondsChanged(int value)
    {
        var action = App.Current.Services.GetService<BlinkerForLaneChangeAction>();
        action!.OffTime = TimeSpan.FromSeconds(value);
    }

}
