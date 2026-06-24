using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Ports;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.ViewModels;

public partial class ViewModel
{
    [ObservableProperty]
    private bool retarderAllReduceActionEnabled;

    partial void OnRetarderAllReduceActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<RetarderAllReduceAction>(oldValue, newValue);

    [ObservableProperty]
    private int retarderAllReduceActionLimitSpeedKph;

    partial void OnRetarderAllReduceActionLimitSpeedKphChanged(int newValue)
    {
        var action = App.Current.Services.GetService<RetarderAllReduceAction>();
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
    private bool retarderAutoOffActionEnabled;

    partial void OnRetarderAutoOffActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<RetarderAutoOffAction>(oldValue, newValue);

    // リターダーを自動的に戻す速度
    [ObservableProperty]
    private int retarderAutoOffActionLimitSpeedKph;

    partial void OnRetarderAutoOffActionLimitSpeedKphChanged(int newValue)
    {
        var action = App.Current.Services.GetService<RetarderAutoOffAction>();
        action!.LimitSpeedKph = newValue;
    }

    // リターダーをスキップ入力する
    [ObservableProperty]
    private bool retarderSkipInputActionEnabled;

    partial void OnRetarderSkipInputActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<RetarderSkipInputAction>(oldValue, newValue);

    // リターダーをスキップ入力するレベル
    [ObservableProperty]
    private int retarderSkipInputLevel;

    partial void OnRetarderSkipInputLevelChanged(int value)
    {
        var action = App.Current.Services.GetService<RetarderSkipInputAction>();
        action!.SkipLevel = value;
    }

    // リターダーをアクセル踏んでるときに全段落とす
    [ObservableProperty]
    private bool retarderAllReduceOnThrottleEnabled;

    partial void OnRetarderAllReduceOnThrottleEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<RetarderAllReduceOnThrottleAction>(oldValue, newValue);
}
