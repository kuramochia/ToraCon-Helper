using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.ViewModels;

public partial class ViewModel
{
    [ObservableProperty]
    private bool autoFullFuelEnabled;

    partial void OnAutoFullFuelEnabledChanged(bool oldValue, bool newValue)  => OnActionEnabledChanged<AutoFullfuelAction>(oldValue, newValue);

    [ObservableProperty]
    private bool followSpeedLimitCruiseControlEnabled;

    partial void OnFollowSpeedLimitCruiseControlEnabledChanged(bool oldValue, bool newValue) => OnActionEnabledChanged<FollowSpeedLimitCruiseControlAction>(oldValue, newValue);


    [ObservableProperty]
    private bool cruiseControlMPHinATS;

    partial void OnCruiseControlMPHinATSChanged(bool value)
    {
        var action = App.Current.Services.GetService<FollowSpeedLimitCruiseControlAction>();
        action!.MphInATS = value;
    }

    [ObservableProperty]
    private int cruiseControlStep;

    partial void OnCruiseControlStepChanged(int value)
    {
        var action = App.Current.Services.GetService<FollowSpeedLimitCruiseControlAction>()!;
        action.CruiseControlStep = value;
    }
}
