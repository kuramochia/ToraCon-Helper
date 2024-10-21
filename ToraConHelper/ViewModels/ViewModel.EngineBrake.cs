using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.ViewModels;

public partial class ViewModel
{
    [ObservableProperty]
    private bool engineBrakeAutoOffActionEnabled;

    partial void OnEngineBrakeAutoOffActionEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<EngineBrakeAutoOffAction>(oldValue, newValue);

    [ObservableProperty]
    private int engineBrakeAutoOffActionLimitSpeedKph;

    partial void OnEngineBrakeAutoOffActionLimitSpeedKphChanged(int value)
    {
        var action = App.Current.Services.GetService<EngineBrakeAutoOffAction>();
        action!.LimitSpeedKph = value;
    }
}
