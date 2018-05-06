using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ConferenceVision.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsView : ContentPage
	{
		public SettingsView()
		{
			InitializeComponent();
		}

		async void Handle_CloseClickedAsync(object sender, System.EventArgs e)
		{
			await Navigation.PopModalAsync(true);
		}
	}
}