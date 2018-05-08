using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using ConferenceVision.Models;
using ConferenceVision.ViewModels;
using Plugin.ImageEdit;
using Xam.Plugins.OnDeviceCustomVision;
using Xamarin.Essentials;
using Xamarin.Forms;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using TrainingTag = Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models.Tag;

namespace ConferenceVision.Services
{
	public class VisionService
	{
		const double PredictionThreshold = 0.75;
		const string ComputerVisionKey = "47577899720d4568ba242f577db496a2";
		const string TrainingKey = "f04976e24cb84498bbad256fc216b701";
		static readonly Dictionary<string, Achievement> Achievements = new AchievementsViewModel().Achievements.ToDictionary(a => a.Name, a => a);
		static readonly AzureRegions CouputerVisionRegion = AzureRegions.Westus2;

		readonly IComputerVisionAPI visionAPI;
		readonly TrainingApi trainingApi;

		public VisionService()
		{
			var creds = new ApiKeyServiceClientCredentials(ComputerVisionKey);
			visionAPI = new ComputerVisionAPI(creds) { AzureRegion = CouputerVisionRegion };
			trainingApi = new TrainingApi() { ApiKey = TrainingKey };
		}

		public Task<bool> DetectAchievements(Memory memory) => ProcessImageFile(memory, GetAchievementsForImage);
		public Task AnalyzeImage(Memory memory) => ProcessImageFile(memory, AnalyzeImageStream);


		public async Task<bool> CreateImagesFromData(Memory memory, IList<string> tags)
		{
			try
			{
				if (!tags.Any())
					return false;

				var project = await trainingApi.GetProjectAsync(new Guid("a7731c4f-8b9f-4012-ab53-98e4e0c48a5b"));
				var existingTags = await trainingApi.GetTagsAsync(project.Id);

				List<string> tagIds = new List<string>();
				foreach (var tag in tags)
				{
					TrainingTag trainingTag = existingTags.FirstOrDefault(x => x.Name == tag);
					if (trainingTag == null)
					{
						try
						{
							trainingTag = await trainingApi.CreateTagAsync(project.Id, tag);
						}
						catch (Microsoft.Rest.HttpOperationException)
						{
							// just in case there is a collision of tags getting created
						}
					}

					if (trainingTag != null)
					{
						tagIds.Add(trainingTag.Id.ToString());
					}
				}

				var fileName = Path.Combine(DependencyService.Get<IMediaFolder>().Path, $"{memory.MediaPath}");
				using (var s = new FileStream(fileName, FileMode.Open))
				{
					var result = await trainingApi.CreateImagesFromDataAsync(project.Id, s, tagIds);
					return result.IsBatchSuccessful;					
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Exception in {nameof(VisionService)}.{nameof(ProcessImageFile)}: {e.Message}");
				Debug.WriteLine(e.StackTrace);
			}

			return false;
		}

		async Task<bool> ProcessImageFile(Memory memory, Func<Memory, Stream, Task> handler)
		{
			try
			{
				var fileName = Path.Combine(DependencyService.Get<IMediaFolder>().Path, $"{memory.MediaPath}");

				using (var s = new FileStream(fileName, FileMode.Open))
				{
					await handler(memory, s);
				}
				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine($"Exception in {nameof(VisionService)}.{nameof(ProcessImageFile)}: {e.Message}");
				Debug.WriteLine(e.StackTrace);
			}

			return false;
		}

		async Task ProcessImageBuffer(byte[] imgBuffer, Memory memory, Func<Memory, Stream, Task> handler)
		{
			using (var s = new MemoryStream(imgBuffer))
			{
				await handler(memory, s);
			}
		}

		async Task GetAchievementsForImage(Memory memory, Stream s)
		{
			var classifications = await CrossImageClassifier.Current.ClassifyImage(s);
			var bestPrediction = classifications.OrderByDescending(c => c.Probability).First();

			if (bestPrediction.Probability < PredictionThreshold)
			{
				// TODO when Xamarin.Essentials adds attachments, we should add this.
				//var result = await App.Current.MainPage.DisplayAlert("No Achievements, No Way!", "We analyzed the photo, but it doesn't look like anything to us. Beg to differ? Email us the photo.", "Okay", "Not Now");
				//if (result == true)
				//{
				//	var msg = new EmailMessage();
				//	msg.To = new List<string> { "david.ortinau@microsoft.com" };
				//	msg.Body = "Attach your photo and tell us what tags (achievements) you think should be here. We'll re-train the Vision model and deploy a new build of the app via AppCenter.";
				//	msg.Subject = "ConferenceVision Photo for Training Set";
				//	await Email.ComposeAsync(msg);

				//}
				return;
			}

			if (Achievements.TryGetValue(bestPrediction.Tag, out var achievement))
			{
				memory.Achievements.Add(achievement);
			}

		}

		async Task AnalyzeImageStream(Memory memory, Stream s)
		{
			try
			{
				if (Connectivity.NetworkAccess != NetworkAccess.Internet)
				{
					await Application.Current.MainPage.DisplayAlert("Not connected", "Please connect to the internet to analyse images", "OK");
					return;
				}

				using (var image = await CrossImageEdit.Current.CreateImageAsync(s))
				{
					var smallerImage = image.Resize(1024);
					var png = smallerImage.ToPng();

					await Task.WhenAll(ProcessImageBuffer(png, memory, GetTagsForImage),
									   ProcessImageBuffer(png, memory, GetNotesForImage));
				}
			}
			catch (Exception e)
			{
				await Application.Current.MainPage.DisplayAlert("Oops!", "Something went wrong, check your internet connection and try again", "OK");

				Debug.WriteLine($"Exception in {nameof(VisionService)}.{nameof(ProcessImageFile)}: {e.Message}");
				Debug.WriteLine(e.StackTrace);
			}
		}

		async Task GetTagsForImage(Memory memory, Stream i)
		{
			var tags = await visionAPI.TagImageInStreamAsync(i);
			memory.Tags = new ObservableCollection<string>(tags.Tags.Select(t => t.Name));
		}

		async Task GetNotesForImage(Memory memory, Stream i)
		{
			var op = await visionAPI.RecognizeTextInStreamAsync(i);
			var id = op.OperationLocation.Split('/').Last();

			TextOperationResult t;
			do
			{
				t = await visionAPI.GetTextOperationResultAsync(id);
			}
			while (t.Status == TextOperationStatusCodes.Running);

			if (t.Status == TextOperationStatusCodes.Succeeded && t.RecognitionResult != null && t.RecognitionResult.Lines != null && t.RecognitionResult.Lines.Any())
			{
				memory.Notes = string.Join("\n", t.RecognitionResult.Lines.Select(l => l.Text));
			}
			else
			{
				memory.Notes = "";
			}
		}
	}
}
