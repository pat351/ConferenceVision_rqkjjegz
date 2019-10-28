using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using ConferenceVision.iOS.Services;
using UIKit;
using Xam.Plugins.OnDeviceCustomVision;
using Xamarin.Forms;

namespace ConferenceVision.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
            // Hello Xamarin.Forms!
            global::Xamarin.Forms.Forms.SetFlags("CarouselView_Experimental");
            global::Xamarin.Forms.Forms.Init();

			// Init 3rd Party Libs
			FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
			FormsCommunityToolkit.Effects.iOS.Effects.Init();
			ImageCircleRenderer.Init();

			// Register Dependencies
			DependencyService.Register<Share>();
			DependencyService.Register<MediaFolder>();
			CrossImageClassifier.Current.Init("VisionModel", ModelType.General);

			// GO!
			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}
