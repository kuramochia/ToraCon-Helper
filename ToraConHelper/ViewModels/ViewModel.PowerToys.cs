using CommunityToolkit.Mvvm.ComponentModel;

namespace ToraConHelper.ViewModels;

public partial class ViewModel
{
	[ObservableProperty]
	private PowerToysViewModel? ets2;

	[ObservableProperty]
	private PowerToysViewModel? ats;
}
