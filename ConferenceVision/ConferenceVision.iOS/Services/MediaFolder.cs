using System;
using ConferenceVision.Services;
using Xamarin.Essentials;

namespace ConferenceVision.iOS.Services
{
    public class MediaFolder : IMediaFolder
    {
        public string Path {
            get{
                return FileSystem.AppDataDirectory;
            }
        }
    }
}
