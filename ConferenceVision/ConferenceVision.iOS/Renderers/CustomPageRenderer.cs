using System;
using ConferenceVision.iOS.Renderers;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(CustomPageRenderer))]
namespace ConferenceVision.iOS.Renderers
{
    public class CustomPageRenderer : PageRenderer
    {
        UIImageView BGImage;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            try
            {
                //var el = (ContentPage)Element;
                var img = UIImage.FromBundle("bg_shadow.png");
                BGImage = new UIImageView(img);
                BGImage.ContentMode = UIViewContentMode.ScaleAspectFill;
                NativeView.InsertSubview(BGImage, 0);


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            BGImage.Frame = NativeView.Bounds;
        }
    }
}