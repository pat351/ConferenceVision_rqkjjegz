using System;
using Android.Content;
using Android.Graphics.Drawables;
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
                var d = Resources.GetDrawable(Resource.Drawable.bg_shadow);
                this.SetBackground(d);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"            ERROR: ", ex.Message);
            }
        }

        //void UpdateBackground(bool setBkndColorEvenWhenItsDefault)
        //{
        //    Page page = Element;
        //    this.SetBackgroundDrawable(Resource.Drawable.bg_shadow);

        //    _ = this.ApplyDrawableAsync(page, Page.BackgroundImageSourceProperty, Context, drawable =>
        //    {
        //        if (drawable != null)
        //        {
        //            this.SetBackground(drawable);
        //        }
        //        else
        //        {
        //            Color bkgndColor = page.BackgroundColor;
        //            bool isDefaultBkgndColor = bkgndColor.IsDefault;
        //            if (page.Parent is BaseShellItem && isDefaultBkgndColor)
        //            {
        //                var color = Forms.IsMarshmallowOrNewer ?
        //                    Context.Resources.GetColor(AColorRes.BackgroundLight, Context.Theme) :
        //                    new AColor(ContextCompat.GetColor(Context, global::Android.Resource.Color.BackgroundLight));
        //                SetBackgroundColor(color);
        //            }
        //            else if (!isDefaultBkgndColor || setBkndColorEvenWhenItsDefault)
        //            {
        //                SetBackgroundColor(bkgndColor.ToAndroid());
        //            }
        //        }
        //    });
        //}

        //public override void ViewDidLayoutSubviews()
        //{
        //    base.ViewDidLayoutSubviews();

        //    BGImage.Frame = NativeView.Bounds;
        //}
    }
}