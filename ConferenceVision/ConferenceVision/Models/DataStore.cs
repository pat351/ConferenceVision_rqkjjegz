using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ConferenceVision.Models
{
	public class DataStore
	{
		public DateTime LastOpened { get; set; } = DateTime.Now;
		public ObservableCollection<Memory> Memories { get; set; } = new ObservableCollection<Memory>();

		public DataStore()
		{
		}
	}
}
