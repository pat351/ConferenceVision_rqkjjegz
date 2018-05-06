using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace ConferenceVision.Views.Renderers
{
	public partial class HyperlinkRowView : FlexLayout
	{
		public static readonly BindableProperty TitleProperty =
			BindableProperty.Create(nameof(Title), typeof(string), typeof(HyperlinkRowView), "My Label");
		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); OnPropertyChanged(nameof(Title)); }
		}

		public static readonly BindableProperty UrlProperty =
			BindableProperty.Create(nameof(Url), typeof(string), typeof(HyperlinkRowView), "");

		public string Url
		{
			get { return (string)GetValue(UrlProperty); }
			set { SetValue(UrlProperty, value); OnPropertyChanged(nameof(Url)); }
		}

		public static readonly BindableProperty IconProperty =
			BindableProperty.Create(nameof(Icon), typeof(string), typeof(HyperlinkRowView), "");
		public string Icon
		{
			get { return (string)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); OnPropertyChanged(nameof(Icon)); }
		}

		public HyperlinkRowView()
		{
			InitializeComponent();
			BindingContext = this;

			this.GestureRecognizers.Add(new TapGestureRecognizer(tappedCallback: OpenBrowserAsync));
		}

		async void OpenBrowserAsync(View arg1, object arg2)
		{
			if (!string.IsNullOrEmpty(Url))
			{
				if (Url.Contains("http"))
				{
					try
					{
						await Xamarin.Essentials.Browser.OpenAsync(this.Url);
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"{ex}");
					}
				}
				else
				{
					await Navigation.PushModalAsync(new HomeworkView() { MarkdownFile = Url }, true);
				}
			}
		}
	}
}
