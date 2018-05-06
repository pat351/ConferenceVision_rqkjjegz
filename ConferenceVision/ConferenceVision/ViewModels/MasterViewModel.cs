using ConferenceVision.Models;
using ConferenceVision.ViewModels.Base;
using ConferenceVision.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ConferenceVision.ViewModels
{
    public class MasterViewModel : ViewModelBase
    {
        public ObservableCollection<MasterViewMenuItem> MenuItems { get; set; }

		public MasterViewModel()
        {
            MenuItems = new ObservableCollection<MasterViewMenuItem>(new[]
            {
                    new MasterViewMenuItem { Id = 0, Title = "Home", TargetType = typeof(HomeView) },
                    new MasterViewMenuItem { Id = 1, Title = "Achievements", TargetType = typeof(AchievementsView) },
                    new MasterViewMenuItem { Id = 2, Title = "My Profile", TargetType = typeof(UserProfileView) },
                    new MasterViewMenuItem { Id = 3, Title = "About", TargetType = typeof(AboutView) },
                });
        }
    }
}
