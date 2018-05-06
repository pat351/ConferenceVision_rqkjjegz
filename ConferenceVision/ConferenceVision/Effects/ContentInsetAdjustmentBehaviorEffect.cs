using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ConferenceVision.Effects
{
	public static class ContentInsetAdjustmentBehavior
	{
		public static readonly BindableProperty ContentInsetProperty =
			BindableProperty.CreateAttached("ContentInset", typeof(Thickness), typeof(ContentInsetAdjustmentBehaviorEffect), new Thickness(0));

		public static Thickness GetContentInset(BindableObject view)
		{
			return (Thickness)view.GetValue(ContentInsetProperty);
		}
	}

	public class ContentInsetAdjustmentBehaviorEffect : RoutingEffect
	{
		public ContentInsetAdjustmentBehaviorEffect() : base("Microsoft.ContentInsetAdjustmentBehaviorEffect")
		{ }
	}
}