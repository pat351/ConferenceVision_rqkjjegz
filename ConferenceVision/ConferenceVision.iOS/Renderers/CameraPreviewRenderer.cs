using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System.Diagnostics;
using AVFoundation;
using System.Threading.Tasks;
using Foundation;
using AssetsLibrary;
using Photos;
using ConferenceVision.Views.Renderers;
using ConferenceVision.iOS.Utils;
using System.IO;
using ConferenceVision.Services;
using CoreGraphics;

// INFO https://github.com/xamarin/xamarin-forms-samples/blob/master/CustomRenderers/ContentPage/iOS/CameraPageRenderer.cs

[assembly: ExportRenderer(typeof(ConferenceVision.Views.Renderers.CameraPreview), typeof(ConferenceVision.iOS.Renderers.CameraPreviewRenderer))]
namespace ConferenceVision.iOS.Renderers
{
    public class CameraPreviewRenderer : ViewRenderer<CameraPreview, UIView>
    {
        string posterPath = String.Empty;
        string albumIdentifier;
        AVCaptureVideoOrientation orientation;
        CameraManager cameraManager = new CameraManager();
        UIView cameraPreview;

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Xamarin.Forms.DesignMode.IsDesignModeEnabled)
            {
                return;
            }

            if (cameraManager != null)
            {
                InitManager();
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        {
            base.OnElementChanged(e);

            if (Xamarin.Forms.DesignMode.IsDesignModeEnabled)
            {
                return;
            }

            if (Element == null) return;

            if (Control == null)
            {
                cameraPreview = new UIView(new CGRect());
                cameraPreview.BackgroundColor = UIColor.Red;
                SetNativeControl(cameraPreview);

                this.BackgroundColor = UIColor.Cyan;

                SetupAlbum();
            }

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {

                e.NewElement.StartRecording = (() => { StartRecording(); });
                e.NewElement.StopRecording = (() => { StopRecording(); });
                e.NewElement.Dispose = (() => { OnDispose(); });
            }


        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (cameraManager != null)
            {
                cameraManager.Filename = Element.Filename;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Debug.WriteLine("Dispose");
            if (disposing)
            {
                if (Control != null)
                {
                    Control.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        public void StartRecording()
        {
            Debug.WriteLine("StartRecording");

            if (cameraManager.OutputMode == CameraOutputMode.StillImage)
            {
                cameraManager.CapturePicture(async (img, err) =>
                {
                    string jpgFilename = System.IO.Path.Combine(
                        DependencyService.Get<IMediaFolder>().Path,
                        $"{Element.Filename}"
                    );
                    NSData imgData = img.AsJPEG();
                    NSError error = null;
                    if (imgData.Save(jpgFilename, false, out err))
                    {
                        Console.WriteLine("saved as " + jpgFilename);
                        FinalizeSave(jpgFilename);
                    }
                    else
                    {
                        Console.WriteLine("NOT saved as " + jpgFilename + " because" + error.LocalizedDescription);
                    }
                });
            }
            else
            {
                cameraManager.startRecordingVideo();
            }
        }

        public void StopRecording()
        {
            Debug.WriteLine("StopRecording");
            cameraManager.stopRecordingVideo(async (url, _orientation, error) =>
            {
                orientation = _orientation;
                SaveToAlbum(url.AbsoluteString);
                Debug.WriteLine("// TODO: SaveToAlbum");
                DeleteOriginal(url);

            });
        }

        void DeleteOriginal(NSUrl url)
        {
            var assets = PHAsset.FetchAssets(new NSUrl[] { url }, null);
            var asset = (PHAsset)assets.firstObject;
            var sharedLib = PHPhotoLibrary.SharedPhotoLibrary;
            sharedLib.PerformChanges(() =>
            {
                var req = asset.CanPerformEditOperation(PHAssetEditOperation.Delete);
                if (req)
                {
                    PHAssetChangeRequest.DeleteAssets(new PHAsset[] { asset });
                }
            }, (success, err) =>
            {
                if (success)
                {
                    Debug.WriteLine("successfully deleted original");
                }
            });
        }

        void SetupAlbum()
        {
            var album = PHAssetCollection.FetchAssetCollections(new[] { Xamarin.Essentials.Preferences.Get("iOSAlbumIdentifier", string.Empty).ToString() }, null)?.firstObject as PHAssetCollection;
            if (album == null)
            {
                var albums = PHAssetCollection.FetchAssetCollections(PHAssetCollectionType.Album, PHAssetCollectionSubtype.Any, new PHFetchOptions
                {
                    IncludeAssetSourceTypes = PHAssetSourceType.UserLibrary
                });
                if (albums.Count > 0)
                {
                    foreach (var a in albums)
                    {
                        var collection = (a as PHAssetCollection);
                        if (collection.LocalizedTitle.Equals("ConferenceVision"))
                        {
                            SaveIdentifier(collection.LocalIdentifier);
                            return;
                        }
                    }
                }
                PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
                {
                    var req = PHAssetCollectionChangeRequest.CreateAssetCollection("ConferenceVision");
                    albumIdentifier = req.PlaceholderForCreatedAssetCollection.LocalIdentifier;
                    SaveIdentifier(albumIdentifier);
                }, (success, error) =>
                {
                    Debug.Write("Create Album " + success);
                });
            }
        }

        void SaveIdentifier(string albumIdentifier)
        {
            Xamarin.Essentials.Preferences.Set("iOSAlbumIdentifier", albumIdentifier);
        }

        void SaveToAlbum(string watermarkedPath)
        {
            var lib = PHPhotoLibrary.SharedPhotoLibrary;
            lib.PerformChanges(() =>
            {
                var album = PHAssetCollection.FetchAssetCollections(new[] { Xamarin.Essentials.Preferences.Get("iOSAlbumIdentifier", string.Empty) }, null)?.firstObject as PHAssetCollection;
                var collectionRequest = PHAssetCollectionChangeRequest.ChangeRequest(album);

                if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
                {
                    var options = new PHAssetResourceCreationOptions
                    {
                        ShouldMoveFile = true
                    };
                    var changeRequest = PHAssetCreationRequest.CreationRequestForAsset();
                    changeRequest.AddResource(PHAssetResourceType.Video, NSUrl.FromString(watermarkedPath), options);

                    collectionRequest.AddAssets(new[] { changeRequest.PlaceholderForCreatedAsset });
                }
                else
                {
                    var changeRequest2 = PHAssetChangeRequest.FromVideo(NSUrl.FromString(watermarkedPath));
                    collectionRequest.AddAssets(new[] { changeRequest2.PlaceholderForCreatedAsset });
                }

                RetrieveLastAssetSaved();

            }, (success, err) =>
            {

            });
        }

        void RetrieveLastAssetSaved()
        {
            var options = new PHFetchOptions();
            options.SortDescriptors = new NSSortDescriptor[]{
                new NSSortDescriptor("creationDate", false)
            };
            options.FetchLimit = 1;

            var result = PHAsset.FetchAssets(PHAssetMediaType.Video, options);
            if (result.firstObject != null)
            {
                var videoAsset = result.firstObject as PHAsset;
                if (videoAsset != null)
                {
                    PHImageManager.DefaultManager.RequestAvAsset(videoAsset, null, (asset, audioMix, info) =>
                    {
                        InvokeOnMainThread(() =>
                        {
                            FinalizeSave(((AVUrlAsset)asset).Url.AbsoluteString);
                        });
                    });
                }
                else
                {
                    Debug.WriteLine("FAIL");
                    //BTProgressHUD.Dismiss();
                }
            }
        }

        void FinalizeSave(string absoluteString)
        {
            Element.OnMediaSaved(
                $"{Element.Filename}",
                posterPath,
                orientation == AVCaptureVideoOrientation.LandscapeLeft || orientation == AVCaptureVideoOrientation.LandscapeRight,
                cameraManager.recordedFileSize
            );

            cameraManager.Dispose();
            cameraManager = null;
            InitManager();
        }

        private ALAssetsGroup _album;
        private void GroupEnumerator(ALAssetsGroup group, ref bool shouldStop)
        {
            if (group != null && group.Name == App.APP_NAME)
            {
                _album = group;
                shouldStop = true;
                return;
            }
            if (group == null)
            {
                shouldStop = true;
                return;
            }
            if (!shouldStop)
            {
                shouldStop = false;
            }
        }

        void InitManager()
        {
            if (cameraManager == null)
            {
                cameraManager = new CameraManager();
            }
            cameraManager.addPreviewLayerToView(Control, CameraOutputMode.StillImage, OnCameraReady);
            cameraManager.Filename = Element.Filename;
            Debug.WriteLine("^^^ NEW CAMERA ^^^");
        }

        private void OnCameraReady()
        {

        }

        public void OnDispose()
        {
            Debug.WriteLine("OnDispose");
            StopRecording();
            Dispose(true);
        }
    }
}