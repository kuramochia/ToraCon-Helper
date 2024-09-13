using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
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
}
