using System;
using Xamarin.Forms;
using ConferenceVision.Services;

namespace ConferenceVision.Converters
{
	public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string source = value as string;
            if (source == null || string.IsNullOrWhiteSpace(source))
                return null;

            var filePath = System.IO.Path.Combine(
                    DependencyService.Get<IMediaFolder>().Path,
                    source);
         	var imageSource = ImageSource.FromFile(filePath);
            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
