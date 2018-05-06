using ConferenceVision.iOS.Renderers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly: ExportRenderer(typeof(ScrollView), typeof(CustomScrollViewRenderer))]
namespace ConferenceVision.iOS.Renderers
{
    public class CustomScrollViewRenderer : ScrollViewRenderer
    {
        
        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;

            var scroll = NativeView as UIScrollView;
            scroll.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Never;
        }

        

    }
}