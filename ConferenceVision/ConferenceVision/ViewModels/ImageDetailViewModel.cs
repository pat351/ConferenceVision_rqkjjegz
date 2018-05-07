using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ConferenceVision.Models;
using ConferenceVision.Services;
using ConferenceVision.ViewModels.Base;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System;

namespace ConferenceVision.ViewModels
{
	public class ImageDetailViewModel : ViewModelBase
	{
		Memory memory;
		public Memory Memory
		{
			get => memory;
			set
			{
				memory = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(VisionNotes));
				OnPropertyChanged(nameof(VisionTags));
			}
		}

		public string ImageSource => memory.MediaPath;
		public string VisionNotes => Memory == null ? "" : Memory.Notes;
		public string VisionTags => Memory == null ? "" : string.Join(" ", Memory.Tags);

		public ObservableCollection<Achievement> Achievements
		{
			get
			{
				return memory.Achievements;
			}
		}

		public ICommand DeleteCommand { get; private set; }
		public ICommand GetVisionResultsCommand { get; private set; }
		public ICommand GetAchievementsCommand { get; private set; }
		public ICommand GoToUrlCommand { get; private set; }

		public ImageDetailViewModel()
		{
			DeleteCommand = new Command(HandleDelete);
			GetVisionResultsCommand = new Command(HandleVision);
			GetAchievementsCommand = new Command(HandleAchievements);
			GoToUrlCommand = new Command<Achievement>(async (model) => await HandleUrl(model));

			if (DesignMode.IsDesignModeEnabled)
			{
				Memory = new Memory
				{
					Notes = "some awesome notes",
					Tags = new ObservableCollection<string> { "one", "two", "three" },
					Achievements = new ObservableCollection<Achievement>{
						new Achievement {
							HasDarkImage = true,
							Name = "Mono",
							Icon = "iconMono"
						},
						new Achievement {
							HasDarkImage = false,
							Name = "Xamarin",
							Icon = "iconXamarin"
						},

					}
				};

			}
		}

		async Task HandleUrl(Achievement model)
		{
			await Browser.OpenAsync(model.Url);
		}

		void HandleDelete(object obj)
		{
			DependencyService.Get<DataStoreService>().DeleteMemory(memory);
		}

		private async void HandleAchievements()
		{
			await DependencyService.Get<VisionService>().DetectAchievements(memory);

			OnPropertyChanged(nameof(Achievements));
			OnPropertyChanged(nameof(HasNoAchievements));
		}

		async void HandleVision(object obj)
		{
			if (DateTime.Today > new DateTime(2018, 5, 13))
			{
				var result = await App.Current.MainPage.DisplayAlert("Vision Key Expired", "Thanks for using the app! To continue using the Vision API, please visit the website and signup for your free trial key.", "Go Now", "Maybe Later");
				if (result == true)
				{
					await Xamarin.Essentials.Browser.OpenAsync("https://azure.microsoft.com/en-us/services/cognitive-services/computer-vision/");
				}

				return;
			}

			IsProcessingVision = true;
			await DependencyService.Get<VisionService>().AnalyzeImage(memory);


			OnPropertyChanged(nameof(VisionNotes));
			OnPropertyChanged(nameof(VisionTags));
			OnPropertyChanged(nameof(HasNoVisionResults));

			var oldItem = App.DataStore.Memories.FirstOrDefault(e => e.Id == memory.Id);

			if (oldItem == null)
			{
				App.DataStore.Memories.Insert(0, memory);
			}
			else
			{
				var index = App.DataStore.Memories.IndexOf(oldItem);
				App.DataStore.Memories.Remove(oldItem);
				App.DataStore.Memories.Insert(index, memory);
			}

			DependencyService.Get<DataStoreService>().Save(App.DataStore);
			IsProcessingVision = false;
		}

		public bool HasNoVisionResults
		{
			get
			{
				if (isProcessingVision)
					return false;

				return (string.IsNullOrEmpty(VisionNotes) && string.IsNullOrEmpty(VisionTags));
			}
		}

		private bool isProcessingVision;
		public bool IsProcessingVision
		{
			get
			{
				return isProcessingVision;
			}
			set
			{
				isProcessingVision = value;
				OnPropertyChanged(nameof(IsProcessingVision));
				OnPropertyChanged(nameof(HasNoVisionResults));
			}
		}

		public bool HasNoAchievements
		{
			get
			{
				return Achievements == null || Achievements.Count == 0;
			}
		}
	}
}