using System;
using System.Collections.Generic;
using ConferenceVision.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ConferenceVision.Views.Renderers
{
	public partial class AchievementView : FlexLayout
	{
		public AchievementView()
		{
			InitializeComponent();
		}

		async void OpenUrl_ClickedAsync(object sender, System.EventArgs e)
		{
			var url = ((Achievement)BindingContext).Url;
			if (!string.IsNullOrEmpty(url))
				await Browser.OpenAsync(url);
		}
	}
}
