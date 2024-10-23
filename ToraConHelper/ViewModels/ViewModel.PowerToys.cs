using CommunityToolkit.Mvvm.ComponentModel;

namespace ToraConHelper.ViewModels;

public partial class ViewModel
{
	[ObservableProperty]
	private PowerToysViewModel? ets2;

    partial void OnEts2Changed(PowerToysViewModel? oldValue, PowerToysViewModel? newValue) => oldValue?.Dispose();

    [ObservableProperty]
	private PowerToysViewModel? ats;

    partial void OnAtsChanged(PowerToysViewModel? oldValue, PowerToysViewModel? newValue) => oldValue?.Dispose();
}
