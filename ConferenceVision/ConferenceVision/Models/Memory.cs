using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ConferenceVision.Models
{
	public class Memory
	{
		public string Id { get; } = Guid.NewGuid().ToString("N");

		public bool Liked { get; set; }
		public string MediaPath { get; set; }
		public ObservableCollection<Achievement> Achievements { get; set; } = new ObservableCollection<Achievement>();
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public string CreatedBy { get; set; }
		public string Notes { get; set; }
		public ObservableCollection<string> Tags { get; set; } = new ObservableCollection<string>();
	}
}
