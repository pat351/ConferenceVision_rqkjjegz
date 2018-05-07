using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Android.Content;
using ConferenceVision.Services;

namespace ConferenceVision.Droid.Services
{
	public class Share : IShare
	{
		private readonly Context _context;
		public Share()
		{
			_context = Android.App.Application.Context;
		}

		Task IShare.Show(string title, string message, string filePath)
		{
			Contract.Ensures(Contract.Result<Task>() != null);
			var extension = filePath.Substring(filePath.LastIndexOf(".", StringComparison.CurrentCulture) + 1).ToLower();
			var contentType = string.Empty;

			// You can manually map more ContentTypes here if you want.
			switch (extension)
			{
				case "pdf":
					contentType = "application/pdf";
					break;
				case "png":
					contentType = "image/png";
					break;
				case "jpg":
					contentType = "image/jpeg";
					break;
				default:
					contentType = "application/octetstream";
					break;
			}

			var uri = Android.Net.Uri.Parse(filePath);
			var intent = new Intent(Intent.ActionSend);
			intent.SetType(contentType);
			intent.PutExtra(Intent.ExtraStream, uri);
			intent.PutExtra(Intent.ExtraText, message ?? string.Empty);
			intent.PutExtra(Intent.ExtraSubject, message ?? string.Empty);

			var chooserIntent = Intent.CreateChooser(intent, title ?? string.Empty);
			chooserIntent.SetFlags(ActivityFlags.ClearTop);
			chooserIntent.SetFlags(ActivityFlags.NewTask);
			_context.StartActivity(chooserIntent);

			return Task.FromResult(true);
		}
	}
}
