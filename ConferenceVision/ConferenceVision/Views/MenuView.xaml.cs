using ConferenceVision.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ConferenceVision.Views
{
	public partial class MenuView : ContentPage
	{
		public ListView ListView;

		public MenuView()
		{
			InitializeComponent();

			BindingContext = new MasterViewModel();
			ListView = menuItemsListView;

		}

		async void DisplayQRShareCode(object sender, System.EventArgs e)
		{
			await Navigation.PushModalAsync(new ShareView(), true);
		}
	}
}