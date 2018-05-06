using System;
using Android.App;
using ConferenceVision.Services;
using Environment = Android.OS.Environment;

namespace ConferenceVision.Droid.Services
{
    public class MediaFolder : IMediaFolder
    {
        public string Path {
			get {
                    return Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;
            }
        }
    }
}
