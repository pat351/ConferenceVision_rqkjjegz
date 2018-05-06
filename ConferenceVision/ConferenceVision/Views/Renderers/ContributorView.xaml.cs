using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ConferenceVision.Views.Renderers
{
	public partial class ContributorView : Grid
	{
		public ContributorView()
		{
			InitializeComponent();
		}

		public static readonly BindableProperty IconProperty =
			BindableProperty.Create(nameof(Icon), typeof(string), typeof(ContributorView), "");
		public string Icon
		{
			get { return (string)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); OnPropertyChanged(nameof(Icon)); }
		}
		public static readonly BindableProperty NameProperty =
			BindableProperty.Create(nameof(Name), typeof(string), typeof(ContributorView), "");

		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); OnPropertyChanged(nameof(Name)); }
		}

		public string Url { get; set; }

		async void OpenUrl_ClickedAsync(object sender, System.EventArgs e)
		{
			if (!string.IsNullOrEmpty(Url))
				await Browser.OpenAsync(Url);
		}
	}
}
