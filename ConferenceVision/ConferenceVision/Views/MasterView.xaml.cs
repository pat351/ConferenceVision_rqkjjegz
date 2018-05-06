using ConferenceVision.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ConferenceVision.Views
{
	public partial class MasterView : MasterDetailPage
	{
		public MasterView()
		{
			InitializeComponent();
			Menu.ListView.ItemSelected += ListView_ItemSelected;
		}

		private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var item = e.SelectedItem as MasterViewMenuItem;
			if (item == null)
				return;

			var page = (Xamarin.Forms.Page)Activator.CreateInstance(item.TargetType);

			Detail = new CustomNavigationPage(page)
			{
				BackgroundColor = Color.Transparent,
				BarBackgroundColor = Color.Transparent
			};
			IsPresented = false;
			Menu.ListView.SelectedItem = null;
		}
	}
}