using ConferenceVision.ViewModels;
using ConferenceVision.Views.Renderers;
using Xamarin.Forms;

namespace ConferenceVision.Views
{
	public partial class AchievementsView : ContentPage
	{
		AchievementsViewModel vm;

		public AchievementsViewModel ViewModel
		{
			get => vm; set
			{
				vm = value;
				BindingContext = vm;
			}
		}

		public AchievementsView()
		{
			InitializeComponent();
			if (vm == null)
				ViewModel = new AchievementsViewModel();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			vm?.OnAppearing();


		}

		async void Handle_InfoClicked(object sender, System.EventArgs e)
		{
			await Navigation.PushModalAsync(new HomeworkView() { MarkdownFile = "HowToGainAchievements.md" }, true);
		}
	}
}