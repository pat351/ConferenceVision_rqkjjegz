using System;
using System.Collections.ObjectModel;
using System.Linq;
using ConferenceVision.Models;
using ConferenceVision.ViewModels.Base;

namespace ConferenceVision.ViewModels
{
	public class AchievementsViewModel : ViewModelBase
	{
		public AchievementsViewModel()
		{
			UpdateUserAchievements();
		}

		void UpdateUserAchievements()
		{
			var memories = App.DataStore.Memories;
			var userAchievements = memories.SelectMany(i => i.Achievements).Distinct();
			foreach (var ua in userAchievements)
			{
				achievements.First(d => d.Name == ua.Name).IsAchieved = true;
			}

			OnPropertyChanged(nameof(Achievements));
		}

		public ObservableCollection<Achievement> Achievements
		{
			get => achievements;
			set => achievements = value;
		}
		private ObservableCollection<Achievement> achievements = new ObservableCollection<Achievement>{
			new Achievement{
				Name = "Xamarin",
				Icon = "iconXamarin",
				Url = "https://docs.microsoft.com/en-us/xamarin/xamarin-forms/"
			},
			new Achievement{
				Name = "Xamarin University",
				Icon = "iconXamarinUniversity",
				Url = "https://university.xamarin.com"
			},
			new Achievement{
				Name = "AppCenter",
				Icon = "iconAppCenter",
				Url = "https://appcenter.ms"
			},
			new Achievement{
				Name = "Azure",
				Icon = "iconAzure",
				Url = "https://azure.microsoft.com/en-us/"
			},
			new Achievement{
				Name = "Azure Functions",
				Icon = "iconAzureFunctions",
				Url = "https://azure.microsoft.com/en-us/services/functions/"
			},
			new Achievement{
				Name = "Mono",
				Icon = "iconMono",
				HasDarkImage = true,
				Url = "https://github.com/mono"
			},
			new Achievement{
				Name = "MvvmCross",
				Icon = "iconMvvmCross",
				HasDarkImage = true,
				Url = "https://www.mvvmcross.com/"
			},
			new Achievement{
				Name = "Prism",
				Icon = "iconPrism",
				HasDarkImage = true,
				Url = "http://prismlibrary.github.io/docs/xamarin-forms/Getting-Started.html"
			},
			new Achievement{
				Name = "Telerik",
				Icon = "iconProgressTelerik",
				Url = "https://www.telerik.com/xamarin-ui"
			},
			new Achievement{
				Name = "VS Code",
				Icon = "iconVSCode",
				Url = "https://code.visualstudio.com"
			},
			new Achievement{
				Name = "Visual Studio",
				Icon = "iconVSIDE",
				Url = "https://www.visualstudio.com"
			},
			new Achievement{
				Name = "VS for Mac",
				Icon = "iconVSMac",
				Url = "https://www.visualstudio.com/vs/mac/"
			},
			new Achievement{
				Name = "VSTS",
				Icon = "iconVSTS",
				Url = "https://www.visualstudio.com/vso/"
			},
			new Achievement{
				Name = "GitHub",
				Icon = "iconGithub",
				HasDarkImage = true,
				Url = "https://github.com"
			},
			new Achievement{
				Name = "Infragistics",
				Icon = "iconInfragistics",
				Url = "https://www.infragistics.com/products/xamarin"
			},
			new Achievement{
				Name = "GrapeCity",
				Icon = "iconGrapeCity",
				HasDarkImage = true,
				Url = "https://www.grapecity.com/en/componentone-xamarin"
			},
			new Achievement{
				Name = "Steema",
				Icon = "iconSteema",
				Url = "https://www.steema.com/product/forms"
			},
			new Achievement{
				Name = "GrialKit",
				Icon = "iconGrial",
				Url = "https://grialkit.com/"
			},
			new Achievement{
				Name = "Gorilla Player",
				Icon = "iconGorilla",
				Url = "https://grialkit.com/gorilla-player/"
			},
			new Achievement{
				Name = "MFractor",
				Icon = "iconMfractor",
				Url = "https://mfractor.com"
			},
			new Achievement{
				Name = "Unity3D",
				Icon = "iconUnity3d",
				HasDarkImage = true,
				Url = "https://unity3d.com"
			},
			new Achievement{
				Name = "MonoGame",
				Icon = "iconMonoGame",
				Url = "https://docs.microsoft.com/en-us/xamarin/graphics-games/monogame/index"
			},
			new Achievement{
				Name = "SkiaSharp",
				Icon = "iconSkia",
				HasDarkImage = true,
				Url = "https://docs.microsoft.com/en-us/xamarin/graphics-games/skiasharp/index"
			},
			new Achievement{
				Name = "CocosSharp",
				Icon = "iconCocos2d",
				Url = "https://docs.microsoft.com/en-us/xamarin/graphics-games/cocossharp/index"
			},
			new Achievement{
				Name = "UrhoSharp",
				Icon = "iconUrho",
				Url = "https://docs.microsoft.com/en-us/xamarin/graphics-games/urhosharp/index"
			},
			new Achievement{
				Name = "MvvmLight",
				Icon = "iconMvvmLight",
				Url = "http://mvvmlight.net"
			},
			new Achievement{
				Name = "ReactiveUI",
				Icon = "iconReactiveUI",
				Url = "https://reactiveui.net/"
			},
			new Achievement{
				Name = "WebAssembly",
				Icon = "iconWebAssembly",
				Url = "http://webassembly.org/"
			},
			new Achievement{
				Name = "Monkey",
				Icon = "iconMonkey",
				Url = "https://docs.microsoft.com/en-us/xamarin/xamarin-forms/"
			},

		};
	}
}
