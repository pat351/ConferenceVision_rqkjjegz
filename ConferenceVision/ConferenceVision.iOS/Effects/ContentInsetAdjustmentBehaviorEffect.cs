using ConferenceVision.iOS.Effects;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("Microsoft")]
[assembly: ExportEffect(typeof(ContentInsetAdjustmentBehaviorEffect), "ContentInsetAdjustmentBehaviorEffect")]
namespace ConferenceVision.iOS.Effects
{
	public class ContentInsetAdjustmentBehaviorEffect : PlatformEffect
	{

		protected override void OnAttached()
		{
			try
			{
				var scroll = Control as UIScrollView;
				scroll.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Never;
				var inset = (Thickness)Element.GetValue(ConferenceVision.Effects.ContentInsetAdjustmentBehavior.ContentInsetProperty);
				scroll.ContentInset = new UIEdgeInsets((nfloat)inset.Top, (nfloat)inset.Left, (nfloat)inset.Bottom, (nfloat)inset.Right);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
			}
		}

		protected override void OnDetached()
		{
		}

		protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
		{
			base.OnElementPropertyChanged(args);

			try
			{
				if (args.PropertyName == "ContentInset")
				{
					var scroll = Control as UIScrollView;
					var inset = (Thickness)Element.GetValue(ConferenceVision.Effects.ContentInsetAdjustmentBehavior.ContentInsetProperty);
					scroll.ContentInset = new UIEdgeInsets((nfloat)inset.Top, (nfloat)inset.Left, (nfloat)inset.Bottom, (nfloat)inset.Right);

				}
			}
			catch (Exception ex)
			{

			}
		}
	}
}