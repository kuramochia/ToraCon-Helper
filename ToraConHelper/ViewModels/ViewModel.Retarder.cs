using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Ports;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.ViewModels;

public partial class ViewModel
{
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

    // リターダーを一段入れたら最大段数まで上げる
    [ObservableProperty]
    private bool retarderFullOnActionEnabled;

    partial void OnRetarderFullOnActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<RetarderFullOnAction>(oldValue, newValue);


    // リターダーをフルから一段下げたら 0 段にする
    [ObservableProperty]
    private bool retarderFullOffActionEnabled;

    partial void OnRetarderFullOffActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<RetarderFullOffAction>(oldValue, newValue);


    // リターダーを自動的に戻す
    [ObservableProperty]
    private bool reterderAutoOffActionEnabled;

    partial void OnReterderAutoOffActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<ReterderAutoOffAction>(oldValue, newValue);

    // リターダーを自動的に戻す速度
    [ObservableProperty]
    private int reterderAutoOffActionLimitSpeedKph;

    partial void OnReterderAutoOffActionLimitSpeedKphChanged(int newValue)
    {
        var action = App.Current.Services.GetService<ReterderAutoOffAction>();
        action!.LimitSpeedKph = newValue;
    }

    // リターダーをスキップ入力する
    [ObservableProperty]
    private bool reterderSkipInputActionEnabled;

    partial void OnReterderSkipInputActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<ReterderSkipInputAction>(oldValue, newValue);

    // リターダーをスキップ入力するレベル
    [ObservableProperty]
    private int reterderSkipInputLevel;

    partial void OnReterderSkipInputLevelChanged(int value)
    {
        var action = App.Current.Services.GetService<ReterderSkipInputAction>();
        action!.SkipLevel = value;
    }

    // リターダーをアクセル踏んでるときに全段落とす
    [ObservableProperty]
    private bool reterderAllReduceOnThrottleEnabled;

    partial void OnReterderAllReduceOnThrottleEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<ReterderAllReduceOnThrottleAction>(oldValue, newValue);
}