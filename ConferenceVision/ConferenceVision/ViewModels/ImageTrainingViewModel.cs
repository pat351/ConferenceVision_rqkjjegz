using ConferenceVision.Models;
using ConferenceVision.Services;
using ConferenceVision.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ConferenceVision.ViewModels
{
	public class ImageTrainingViewModel : ViewModelBase
	{
		static readonly IList<Achievement> AvailableAchievements = new AchievementsViewModel().Achievements.ToList();
		private readonly Memory _memory;

		public ImageTrainingViewModel(Memory memory)
		{
			AvailableAchievementsSource = 
				AvailableAchievements
					.Select(a => new SelectableData() { Data = a, Selected = memory.Achievements.Where(x=> x.IsAchieved).Any(ma => ma.Name == a.Name)})
					.OrderBy(x=> x.Data.Name)
					.ToList();

			_memory = memory;
		}

		public async Task HandleTrainingCustomVisionAsync()
		{
			var tags = AvailableAchievementsSource.Where(x => x.Selected).Select(x => x.Data.Name).ToList();
			await DependencyService
				.Get<VisionService>()
				.CreateImagesFromData(_memory, tags);
		}

		public IList<SelectableData> AvailableAchievementsSource { get; } 

		public class SelectableData
		{
			public Achievement Data { get; set; }
			public bool Selected { get; set; }
		}
	}
}
