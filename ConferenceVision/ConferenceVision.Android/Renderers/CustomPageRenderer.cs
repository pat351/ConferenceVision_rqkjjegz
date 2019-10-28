using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.V4.Content.Res;
using ConferenceVision.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ContentPage), typeof(CustomPageRenderer))]
namespace ConferenceVision.Droid.Renderers
{
    public class CustomPageRenderer : PageRenderer
    {
        Drawable BGImage;

        public CustomPageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            try
            {
                var d = ResourcesCompat.GetDrawable(Resources, Resource.Drawable.bg_shadow, null);
                this.SetBackground(d);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
            }
        }
    }
}