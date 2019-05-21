using System;
using System.Collections.Generic;
using ConferenceVision.Views;
using Xamarin.Forms;

namespace ConferenceVision
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        async void DisplayQRShareCode(object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new ShareView(), true);
        }
    }
}
