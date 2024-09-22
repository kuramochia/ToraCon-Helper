using CommunityToolkit.Mvvm.ComponentModel;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.ViewModels;

public partial class ViewModel
{
    [ObservableProperty]
    private bool autoFullFuelEnabled;

    partial void OnAutoFullFuelEnabledChanged(bool oldValue, bool newValue)  => OnActionEnabledChanged<AutoFullfuelAction>(oldValue, newValue);

}
