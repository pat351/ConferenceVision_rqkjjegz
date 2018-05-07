using ConferenceVision.Models;
using ConferenceVision.Services;
using ConferenceVision.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ConferenceVision
{
	public partial class App : Application
	{
		public const string APP_NAME = "ConferenceVision";

		public static DataStore DataStore;

		public App()
		{
#if DEBUG
			/*
             * Live Reload Preview is Windows only. On macOS it'll do nothing.
             * 1) Install the VS extension and follow the instructions here https://marketplace.visualstudio.com/items?itemName=Xamarin.XamarinLiveReload
            */
			try
			{
				LiveReload.Init();
			}
			catch { }
#endif
			InitializeComponent();

			InitDependencies();
			InitData();

			MainPage = new MasterView();
		}

		void InitDependencies()
		{
			DependencyService.Register<DataStoreService>();
			DependencyService.Register<VisionService>();
		}

		void InitData()
		{
			DependencyService.Get<DataStoreService>().Load(App.DataStore);
		}

		protected override void OnStart()
		{
			// Handle when your app starts
			AppCenter.Start("ios=14cc1c7a-ae7d-4545-b1bb-e7d2a7d5bafc;" + "uwp=068b85e2-6232-46b8-8a1b-0158efc65277;" + "android=36de2c1e-c507-4016-9df2-04c4b1e303de", typeof(Analytics), typeof(Crashes), typeof(Distribute));
		}

		protected override void OnSleep()
		{
			MessagingCenter.Send<App>(this, nameof(OnSleep));

		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
