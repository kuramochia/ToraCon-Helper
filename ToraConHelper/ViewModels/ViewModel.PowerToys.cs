using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToraConHelper.ViewModels;

public partial class ViewModel
{
	[ObservableProperty]
	private PowerToysViewModel? ets2;

	[ObservableProperty]
	private PowerToysViewModel? ats;
}
