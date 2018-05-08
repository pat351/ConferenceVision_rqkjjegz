using ConferenceVision.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ConferenceVision.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ImageTrainingView : ContentPage
	{
		public ImageTrainingView ()
		{
			InitializeComponent ();
			NavigationPage.SetHasNavigationBar(this, true);
			NavigationPage.SetBackButtonTitle(this, string.Empty);
		}

		private async void Handle_SendToVisionClicked(object sender, EventArgs e)
		{
			try
			{
				var vm = BindingContext as ImageTrainingViewModel;
				ActivitySpinner.IsVisible = true;
				ActivitySpinner.IsRunning = true;
				SubmitToVision.IsVisible = false;

				// don't hold up the UI while data is processed
				Task.Run(async () =>
				{
					try
					{
						await vm.HandleTrainingCustomVisionAsync();
					}
					catch (Exception exc)
					{
						Debug.WriteLine($"HandleTrainingCustomVisionAsync Failed {exc}");
					}
				});
			}
			catch(Exception exc)
			{
				Debug.WriteLine($"Handle_SendToVisionClicked Failed {exc}");
			}
			finally
			{
				await DisplayAlert("Thank You", "Thank you for helping improve our Custom Vision service. We've added your photo to our training set", "I'm Awesome");
				await Navigation.PopAsync(true);
			}
		}
	}
}