using System;
using Xamarin.Forms;
using ConferenceVision.Services;
using System.Diagnostics;

namespace ConferenceVision.Converters
{
	public class AchievedOpacityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var achieved = (bool)value;
			return achieved ? 1 : 0.2;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
