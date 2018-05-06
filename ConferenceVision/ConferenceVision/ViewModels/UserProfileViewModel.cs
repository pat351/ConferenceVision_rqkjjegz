using ConferenceVision.Models;
using ConferenceVision.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ConferenceVision.ViewModels
{
	public class UserProfileViewModel : ViewModelBase
	{
		public UserProfileViewModel()
		{
		}

		public List<Memory> Memories
        {
            get
            {
                return App.DataStore.Memories.Take(10).ToList();
            }
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
