using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Permissions;
using Plugin.CurrentActivity;
using FFImageLoading.Forms.Droid;
using Xamarin.Forms;
using ConferenceVision.Droid.Services;
using Xamarin.Forms.Platform.Android;
using Android.Util;

namespace ConferenceVision.Droid
{
	[Activity(Label = "Vision",
			  Theme = "@style/MainTheme",
			  MainLauncher = false,
			  ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

            // Hello Xamarin.Forms!
            global::Xamarin.Forms.Forms.SetFlags("CarouselView_Experimental");
            global::Xamarin.Forms.Forms.Init(this, bundle);

			// Init 3rd Party Libs
			CachedImageRenderer.Init(enableFastRenderer: true);
			FormsCommunityToolkit.Effects.Droid.Effects.Init();
			ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init();
			Xam.Plugins.OnDeviceCustomVision.CrossImageClassifier.Current.Init("model.pb", Xam.Plugins.OnDeviceCustomVision.ModelType.General);

			// Register Dependencies
			DependencyService.Register<Share>();
			DependencyService.Register<MediaFolder>();
			MakeStatusBarTranslucent(false);

			// GO!
			LoadApplication(new App());
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		private void MakeStatusBarTranslucent(bool makeTranslucent)
		{
			if (makeTranslucent)
			{
				SetStatusBarColor(Android.Graphics.Color.Transparent);

				if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
				{
					Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(SystemUiFlags.LayoutFullscreen | SystemUiFlags.LayoutStable);
				}
			}
			else
			{
				using (var value = new TypedValue())
				{
					if (Theme.ResolveAttribute(Resource.Attribute.colorPrimaryDark, value, true))
					{
						var color = new Android.Graphics.Color(value.Data);
						SetStatusBarColor(color);
					}
				}

				if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
				{
					Window.DecorView.SystemUiVisibility = StatusBarVisibility.Visible;
				}
			}
		}

	}
}

