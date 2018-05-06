using ConferenceVision.Models;
using ConferenceVision.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ConferenceVision.ViewModels
{
	public class HomeViewModel : ViewModelBase
	{
		public HomeViewModel()
		{
		}

		public ObservableCollection<Memory> Memories
		{
			get => App.DataStore.Memories;
		}

		public override void OnAppearing()
		{
			base.OnAppearing();

			OnPropertyChanged(nameof(Memories));
			OnPropertyChanged(nameof(HasNoMemories));
		}

		public bool HasNoMemories
		{
			get
			{
				return this.Memories == null || this.Memories.Count == 0;
			}
		}
	}
}
