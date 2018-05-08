namespace ConferenceVision.Models
{
	public class Achievement
	{
		public string Name { get; set; }
		public string DarkIcon { get => (HasDarkImage) ? $"{Icon}Dark" : Icon; }
		public string Icon { get; set; }
		public bool IsAchieved { get; set; }
		public string Url { get; set; }
		public bool HasDarkImage { get; set; }		
	}
}