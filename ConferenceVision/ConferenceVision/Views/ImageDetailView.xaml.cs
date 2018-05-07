using Xamarin.Forms;
using ConferenceVision.ViewModels;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using ConferenceVision.Services;
using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace ConferenceVision.Views
{
	public partial class ImageDetailView : ContentPage
	{
		private ImageDetailViewModel vm;

		public ImageDetailViewModel VM
		{
			get => vm; set
			{
				vm = value;
				BindingContext = vm;
			}
		}

		void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
		{
			var w = args.Info.Size.Width;
			var h = args.Info.Size.Height;
			var surface = args.Surface;
			var canvas = surface.Canvas;
			canvas.Clear(SKColors.Transparent);

			var paint = new SKPaint
			{
				IsAntialias = true,
				Style = SKPaintStyle.Fill,
				Color = SKColor.Parse("#FFFFFF"),
				StrokeWidth = 0
			};

			var path = new SKPath { FillType = SKPathFillType.EvenOdd };
			path.MoveTo(0, h);
			path.LineTo(w, h);
			path.LineTo(w, 0);
			path.LineTo(0, h);
			path.Close();
			canvas.DrawPath(path, paint);
		}

		void Handle_ShareClicked(object sender, System.EventArgs e)
		{
			var share = DependencyService.Get<IShare>();
			share.Show("Share",
					   "Check out this photo I took at #build2018 with the #XamarinForms + #vision_api app I made. Get the app and code: https://aka.ms/cv-app",
					   System.IO.Path.Combine(
							DependencyService.Get<IMediaFolder>().Path,
							vm.ImageSource)
					  );
		}

		public ImageDetailView()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.Padding = 0;

			NavigationPage.SetBackButtonTitle(this, string.Empty);

			if (DesignMode.IsDesignModeEnabled)
			{
				VM = new ImageDetailViewModel();
			}

		}

		async void Handle_DeleteClickedAsync(object sender, System.EventArgs e)
		{
			VM.DeleteCommand.Execute(null);
			await Navigation.PopAsync();
		}
	}
}