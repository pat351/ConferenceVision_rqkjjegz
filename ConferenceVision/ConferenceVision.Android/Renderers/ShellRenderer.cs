using System;
using Android.Content;
using Android.Content.Res;
using Android.Support.Design.Widget;
using Android.Support.V4.Content.Res;
using Android.Support.V7.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Shell), typeof(ConferenceVision.Droid.Renderers.ShellRenderer))]
namespace ConferenceVision.Droid.Renderers
{
    public class ShellRenderer : Xamarin.Forms.Platform.Android.ShellRenderer
    {
        Context context;

        public ShellRenderer(Context ctx) : base(ctx)
        {
            context = ctx;
        }

        protected override IShellFlyoutContentRenderer CreateShellFlyoutContentRenderer()
        {
            var flyout = base.CreateShellFlyoutContentRenderer();

            var bg = ResourcesCompat.GetDrawable(context.Resources, Resource.Drawable.bg_seattle, null);
            //flyout.AndroidView.SetBackground(drawable: bg);

            var cl = ((CoordinatorLayout)flyout.AndroidView);
            //cl.SetBackgroundColor(Color.PeachPuff.ToAndroid());
            cl.SetBackground(bg);

            var g = (AppBarLayout)cl.GetChildAt(0);
            g.SetBackgroundColor(Color.Transparent.ToAndroid());
            g.OutlineProvider = null;

            var header = g.GetChildAt(0);
            header.SetBackgroundColor(Color.Transparent.ToAndroid());


            return flyout;
        }

        protected override IShellToolbarTracker CreateTrackerForToolbar(Toolbar toolbar)
        {

            var d = ResourcesCompat.GetDrawable(context.Resources, Resource.Drawable.toolbar_bg, null);
            toolbar.SetBackground(d);

            var t = base.CreateTrackerForToolbar(toolbar);
            return t;
        }
    }
}
