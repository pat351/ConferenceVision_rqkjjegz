using ConferenceVision.Models;
using ConferenceVision.Services;
using ConferenceVision.ViewModels.Base;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Xamarin.Forms;

namespace ConferenceVision.ViewModels
{
	public class HomeViewModel : ViewModelBase
	{
		public HomeViewModel()
		{
			PickPhotoCommand = new Command(HandlePickPhoto);
            ItemTappedCommand = new Command<Memory>(HandleItemTapped);

        }

        private async void HandleItemTapped(Memory memory)
        {
            await Shell.Current.GoToAsync($"details?id={memory.Id}");
        }

        public Command PickPhotoCommand { get; }

        public Command ItemTappedCommand { get; }

        async void HandlePickPhoto()
		{
			var file = await CrossMedia.Current.PickPhotoAsync();
			if (file != null)
			{
				var parts = file.Path.Split('/');
				var filename = parts[parts.Length - 1];
				var dest = Path.Combine(
					DependencyService.Get<IMediaFolder>().Path,
					filename);

				File.Copy(file.Path, dest);

				var m = new Memory
				{
					MediaPath = filename
				};

				if (await DependencyService.Get<VisionService>().DetectAchievements(m))
				{
					App.DataStore.Memories.Insert(0, m);

					DependencyService.Get<DataStoreService>().Save(App.DataStore);

					OnPropertyChanged(nameof(Memories));
					OnPropertyChanged(nameof(HasNoMemories));
				}
				else
				{
					await App.Current.MainPage.DisplayAlert("Error", "Failed to Save Image", "Cancel");
				}
			}
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
