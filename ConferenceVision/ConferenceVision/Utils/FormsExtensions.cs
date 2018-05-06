using System;
using Xamarin.Forms;

namespace ConferenceVision.Utils
{
	public static class FormsExtensions
	{


		public static void DeselectOnTap(this ListView listView)
		{
			listView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) =>
			{
				listView.SelectedItem = null;
			};
		}
	}
}
