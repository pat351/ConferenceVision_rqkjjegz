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
	public partial class ImageTrainingView : ContentPage
	{
		ImageTrainingViewModel vm;

		public ImageTrainingViewModel ViewModel
		{
			get => vm; set
			{
				vm = value;
				BindingContext = vm;
			}
		}

		public ImageTrainingView()
		{
			InitializeComponent();

			NavigationPage.SetHasNavigationBar(this, true);
			NavigationPage.SetBackButtonTitle(this, string.Empty);

			if (DesignMode.IsDesignModeEnabled)
				ViewModel = new ImageTrainingViewModel(new Models.Memory());
		}

		private async void Handle_SendToVisionClicked(object sender, EventArgs e)
		{
			try
			{

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
			catch (Exception exc)
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