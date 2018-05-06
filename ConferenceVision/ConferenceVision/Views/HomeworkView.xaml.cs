using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xam.Forms.Markdown;
using Xamarin.Forms;

namespace ConferenceVision.Views
{
	public partial class HomeworkView : ContentPage, INotifyPropertyChanged
	{
		public HomeworkView()
		{
			InitializeComponent();
			BindingContext = this;

			MDView.Theme = new DarkMarkdownTheme();
		}


		string markdownContent;
		public string MarkdownContent
		{
			get
			{
				return markdownContent;
			}
		}

		public string MarkdownFile
		{
			set
			{
				markdownContent = GetText(value);
				OnPropertyChanged(nameof(MarkdownContent));
			}
		}

		string GetText(string filename)
		{
			var assembly = IntrospectionExtensions.GetTypeInfo(typeof(HomeworkView)).Assembly;
			Stream stream = assembly.GetManifestResourceStream($"ConferenceVision.Resources.{filename}");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}

			return text;
		}

		async void OnCloseAsync(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync(true);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
		 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	}
}
