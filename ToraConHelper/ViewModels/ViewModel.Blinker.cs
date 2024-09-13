using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
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
}
