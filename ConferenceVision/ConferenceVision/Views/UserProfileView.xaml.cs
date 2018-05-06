using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConferenceVision.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ConferenceVision.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserProfileView : ContentPage
	{
		UserProfileViewModel vm;
		public UserProfileViewModel ViewModel
		{
			get => vm; set
			{
				vm = value;
				BindingContext = vm;
			}
		}

		public UserProfileView()
		{
			InitializeComponent();
			if (vm == null)
				ViewModel = new UserProfileViewModel();
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

		protected override void OnAppearing()
		{
			base.OnAppearing();

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            //// Sure, this is convoluted. We want to preserve the labels and box view, 
            //// then replace all else. We could replace all images as well, or have a bindable container.                
            //while (FlexContainer.Children.Count > 3)
            //    FlexContainer.Children.RemoveAt(3);

            //if (vm.HasNoMemories)
            //{
            //    // Ideally we'd allow the user to go directly to the camera here. 
            //    // Advanced Challenge: Refactor the camera and permissions code to make it portable
            //    var btn = new Button
            //    {
            //        Text = "Take a Photo",
            //        Style = Application.Current.Resources["OutlineButtonDark"] as Style,
            //        Margin = new Thickness(15)
            //    };
            //    btn.Clicked += UserProfileView_Clicked;

            //    // add a button to take a photo
            //    FlexContainer.Children.Add(
            //        btn
            //    );
            //}
            //else
            //{
            //    var imageSourceConverter = new Converters.ImageSourceConverter();
            //    // add the images
            //    foreach(var memory in vm.Memories)
            //    {
            //        FlexContainer.Children.Add(
            //            new Image
            //            {
            //                Source = (ImageSource)imageSourceConverter.Convert(memory.MediaPath, typeof(ImageSource),null,null)
            //            }
            //        );
            //    }
            //}
        }

		void UserProfileView_Clicked(object sender, EventArgs e)
		{
			((MasterDetailPage)App.Current.MainPage).Detail =
				new CustomNavigationPage(
					new HomeView()
				)
				{
					BackgroundColor = Color.Transparent,
					BarBackgroundColor = Color.Transparent
				}
			;
		}

		async void GoToXamarinBooth(object sender, System.EventArgs e)
		{
			await Navigation.PushModalAsync(new HomeworkView() { MarkdownFile = "XamarinBooth.md" }, true);
		}
	}
}