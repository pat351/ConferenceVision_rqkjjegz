using System;
using Foundation;
using AVFoundation;
using UIKit;
using System.Linq;
using CoreMedia;
using AssetsLibrary;
using System.IO;
using System.Diagnostics;
using CoreFoundation;
using CoreGraphics;
using static UIKit.UIImage;
using Photos;

namespace ConferenceVision.iOS.Utils
{
	public enum CameraState
	{
		Ready, AccessDenied, NoDeviceFound, NotDetermined
	}

	public enum CameraDevice
	{
		Front, Back
	}

	public enum CameraFlashMode : int
	{
		Off = 0, On = 1, Auto = 2
	}

	public enum CameraOutputMode
	{
		StillImage, VideoWithMic, VideoOnly
	}

	public enum CameraOutputQuality : int
	{
		Low = 0, Medium = 1, High = 2
	}

	public class CameraManager : NSObject, IAVCaptureFileOutputRecordingDelegate
	{
		// MARK: - Public properties

		/// Capture session to customize camera settings.
		public AVCaptureSession captureSession;

		/// Property to determine if the manager should show the error for the user. If you want to show the errors yourself set this to false. If you want to add custom error UI set showErrorBlock property. Default value is false.
		public bool showErrorsToUsers = false;

		/// Property to determine if the manager should show the camera permission popup immediatly when it's needed or you want to show it manually. Default value is true. Be carful cause using the camera requires permission, if you set this value to false and don't ask manually you won't be able to use the camera.
		public bool showAccessPermissionPopupAutomatically = true;

		/// A block creating UI to present error message to the user. This can be customised to be presented on the Window root view controller, or to pass in the viewController which will present the UIAlertController, for example.
		//        public void showErrorBlock(string erTitle, string erMessage) { 
		//            (erTitle: String, erMessage: String) -> Void in
		//
		//        
		//        }

		/// Property to determine if manager should write the resources to the phone library. Default value is true.
		public bool writeFilesToPhoneLibrary = true;

		public bool _shouldRespondToOrientationChanges = true;
		public bool ShouldRespondToOrientationChanges
		{
			get
			{
				return _shouldRespondToOrientationChanges;
			}
			set
			{
				_shouldRespondToOrientationChanges = value;
				if (_shouldRespondToOrientationChanges)
				{
					_startFollowingDeviceOrientation();
				}
				else
				{
					_stopFollowingDeviceOrientation();
				}
			}
		}

		public bool cameraIsReady
		{
			get
			{
				return cameraIsSetup;
			}
		}

		public bool hasFrontCamera
		{
			get
			{
				var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
				return devices.Any(x => x.Position == AVCaptureDevicePosition.Front);
			}
		}

		public bool hasFlash
		{
			get
			{
				var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
				var back = devices.First(x => x.Position == AVCaptureDevicePosition.Back);
				return back.HasFlash;
			}
		}

		private CameraDevice _cameraDevice = CameraDevice.Back;
		public CameraDevice CameraDevice
		{
			get
			{
				return _cameraDevice;
			}
			set
			{

				if (cameraIsSetup)
				{
					if (_cameraDevice != value)
					{
						_cameraDevice = value;
						_updateCameraDevice(_cameraDevice);
						_setupMaxZoomScale();
						_zoom(0);
					}
				}
			}
		}

		private CameraFlashMode _flashMode = CameraFlashMode.Off;
		public CameraFlashMode FlashMode
		{
			get
			{
				return _flashMode;
			}
			set
			{
				if (cameraIsSetup)
				{
					if (_flashMode != value)
					{
						_flashMode = value;
						_updateFlasMode(value);
					}
				}
			}
		}

		private CameraOutputQuality _cameraOutputQuality = CameraOutputQuality.High;
		public CameraOutputQuality CameraOutputQuality
		{
			get
			{
				return _cameraOutputQuality;
			}
			set
			{
				if (cameraIsSetup)
				{
					if (_cameraOutputQuality != value)
					{
						_cameraOutputQuality = value;
						_updateCameraQualityMode(_cameraOutputQuality);
					}
				}
			}
		}

		/// Property to change camera output.
		private CameraOutputMode _outputMode = CameraOutputMode.StillImage;
		public CameraOutputMode OutputMode
		{
			get
			{
				return _outputMode;
			}
			set
			{
				if (cameraIsSetup)
				{
					if (value != _outputMode)
					{
						_setupOutputMode(value, _outputMode);
						_outputMode = value;
					}
					_setupMaxZoomScale();
					_zoom(0);
				}
			}
		}

		public CMTime recordedDuration
		{
			get
			{
				if (movieOutput != null)
				{
					return movieOutput.RecordedDuration;
				}
				else
				{
					return CMTime.Zero;
				}
			}
		}

		public Int64 recordedFileSize
		{
			get
			{
				if (movieOutput != null)
				{
					return movieOutput.RecordedFileSize;
				}
				else
				{
					return 0;
				}
			}
		}

		private UIView embeddingView;

		private Action<NSUrl, AVCaptureVideoOrientation, NSError> videoCompletion;

		// TODO not sure what to do with a queue
		//        private var sessionQueue: dispatch_queue_t = dispatch_queue_create("CameraSessionQueue", DISPATCH_QUEUE_SERIAL)

		private AVCaptureDevice frontCameraDevice
		{
			get
			{
				var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
				return devices.FirstOrDefault(x => x.Position == AVCaptureDevicePosition.Front);
			}
		}

		private AVCaptureDevice backCameraDevice
		{
			get
			{
				var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
				return devices.FirstOrDefault(x => x.Position == AVCaptureDevicePosition.Back);
			}
		}

		private AVCaptureDevice mic
		{
			get
			{
				var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Audio);
				return devices.FirstOrDefault();
			}
		}

		private AVCaptureStillImageOutput stillImageOutput;
		private AVCaptureMovieFileOutput movieOutput;
		private AVCaptureVideoPreviewLayer previewLayer;
		private ALAssetsLibrary library;

		private bool cameraIsSetup = false;
		private bool cameraIsObservingDeviceOrientation = false;

		private nfloat zoomScale = 1.0f;
		private nfloat beginZoomScale = 1.0f;
		private nfloat maxZoomScale = 1.0f;

		private string _filename = "tempMovie.mp4";
		public string Filename
		{
			get
			{
				return _filename;
			}
			set
			{
				_filename = value;
				Debug.WriteLine("_filename {0}", _filename);
			}
		}

		private NSUrl tempFilePath
		{
			get
			{
				var tempPath = NSUrl.FromFilename(Path.Combine(Path.GetTempPath(), _filename));
				if (NSFileManager.DefaultManager.FileExists(tempPath.Path))
				{
					NSError error;
					NSFileManager.DefaultManager.Remove(tempPath, out error);
				}
				return tempPath;
			}
		}

		public CameraState addPreviewLayerToView(UIView view)
		{
			return addPreviewLayerToView(view, OutputMode);
		}

		public CameraState addPreviewLayerToView(UIView view, CameraOutputMode newCameraOutputMode)
		{
			return addPreviewLayerToView(view, newCameraOutputMode, null);
		}

		public CameraState addPreviewLayerToView(UIView view, CameraOutputMode newCameraOutputMode, Action completion)
		{
			if (_canLoadCamera())
			{
				if (embeddingView != null)
				{
					if (previewLayer != null)
					{
						previewLayer.RemoveFromSuperLayer();
					}
				}

				if (cameraIsSetup)
				{
					_addPreviewLayerToView(view);
					OutputMode = newCameraOutputMode;
					if (completion != null)
					{
						completion();
					}
				}
				else
				{
					_setupCamera(
						() =>
						{
							this._addPreviewLayerToView(view);
							this.OutputMode = newCameraOutputMode;
							if (completion != null)
							{
								completion();
							}
						}
					);
				}
			}
			return _checkIfCameraIsAvailable();
		}

		public void askUserForCameraPermissions(Action<bool> completion)
		{
			AVCaptureDevice.RequestAccessForMediaType(AVMediaType.Video, allowed =>
			{
				if (this.OutputMode == CameraOutputMode.VideoWithMic)
				{
					AVCaptureDevice.RequestAccessForMediaType(AVMediaType.Audio, audioAllowed =>
					{
						completion(audioAllowed);
					});
				}
				else
				{
					completion(allowed);
				}
			});
		}

		public void stopCaptureSession()
		{
			if (captureSession != null)
				captureSession.StopRunning();
			_stopFollowingDeviceOrientation();
		}

		public void resumeCaptureSession()
		{
			if (captureSession != null)
			{
				if (!captureSession.Running && cameraIsSetup)
				{
					captureSession.StartRunning();
					_startFollowingDeviceOrientation();
				}
			}
			else
			{
				if (_canLoadCamera())
				{
					if (cameraIsSetup)
					{
						stopAndRemoveCaptureSession();
					}
					_setupCamera(() =>
					{
						if (this.embeddingView != null)
						{
							this._addPreviewLayerToView(embeddingView);
						}
						this._startFollowingDeviceOrientation();
					});
				}
			}
		}

		public void stopAndRemoveCaptureSession()
		{
			stopCaptureSession();
			_cameraDevice = CameraDevice.Back;
			cameraIsSetup = false;
			previewLayer = null;
			captureSession = null;
			//            frontCameraDevice = null;
			//            backCameraDevice = null;
			//            mic = null;
			stillImageOutput = null;
			movieOutput = null;
			embeddingView = null;
		}

		public void CapturePicture(Action<UIImage, NSError> completion)
		{
			if (!cameraIsSetup)
			{
				return;
			}

			if (_outputMode != CameraOutputMode.StillImage)
			{
				return;
			}

			NSData imageData;
			_getStillImageOutput().CaptureStillImageAsynchronously(
				stillImageOutput.ConnectionFromMediaType(AVMediaType.Video),
				(imageDataSampleBuffer, error) =>
				{
					if (imageDataSampleBuffer == null)
					{
						return;
					}

					imageData = AVCaptureStillImageOutput.JpegStillToNSData(imageDataSampleBuffer);

					var image = new UIImage(imageData);
					image.SaveToPhotosAlbum((img, err) =>
					{
						completion(img, err);
					});
				});
		}

		public void startRecordingVideo()
		{
			Debug.Print("tempFilePath {0}", tempFilePath);
			_getMovieOutput().StartRecordingToOutputFile(tempFilePath, this);

		}

		/**
        Stop recording a video. Save it to the cameraRoll and give back the url.
        */
		public void stopRecordingVideo(Action<NSUrl, AVCaptureVideoOrientation, NSError> completion)
		{
			if (movieOutput != null)
			{
				if (movieOutput.Recording)
				{
					videoCompletion = completion;
					movieOutput.StopRecording();
				}
			}
		}

		public CameraState currentCameraStatus()
		{
			return _checkIfCameraIsAvailable();
		}

		public void captureOutput(AVCaptureFileOutput captureOutput, NSUrl fileURL)
		{
			if (captureSession != null)
				captureSession.BeginConfiguration();

			if (_flashMode != CameraFlashMode.Off)
			{
				_updateTorch(_flashMode);
			}
			if (captureSession != null)
				captureSession.CommitConfiguration();
		}

		public void captureOutput(AVCaptureFileOutput captureOutput, NSUrl outputFileURL, NSError error = null)
		{
			_updateTorch(CameraFlashMode.Off);
			var orientation = _currentVideoOrientation();
			if (library != null)
			{
				if (writeFilesToPhoneLibrary)
				{
					library.WriteVideoToSavedPhotosAlbum(outputFileURL, (assetUrl, err) =>
					{
						if (err != null)
						{
							this._executeVideoCompletionWithURL(null, orientation, err);
						}
						else
						{
							if (assetUrl != null)
							{
								this._executeVideoCompletionWithURL(assetUrl, orientation, err);
							}
						}
					});
				}
				else
				{
					_executeVideoCompletionWithURL(outputFileURL, orientation, null);
				}
			}
		}

		private void attachZoom(UIView view)
		{
			var pinch = new UIPinchGestureRecognizer(_zoomStart);
			view.AddGestureRecognizer(pinch);
			pinch.Delegate = new GestureDelegate(); ;
		}

		public bool gestureRecognizerShouldBegin(UIGestureRecognizer gestureRecognizer)
		{
			if (gestureRecognizer.GetType() == typeof(UIPinchGestureRecognizer))
			{
				beginZoomScale = zoomScale;
			}

			return true;
		}

		private void _zoomStart(UIPinchGestureRecognizer recognizer)
		{
			if (embeddingView == null || previewLayer == null)
				return;

			var allTouchesOnPreviewLayer = true;
			var numTouch = recognizer.NumberOfTouches;

			for (int i = 0; i < numTouch; i++)
			{
				var location = recognizer.LocationOfTouch(i, embeddingView);
				var convertedTouch = previewLayer.ConvertPointFromLayer(location, previewLayer.SuperLayer);
				if (!previewLayer.Contains(convertedTouch))
				{
					allTouchesOnPreviewLayer = false;
					break;
				}
			}
			if (allTouchesOnPreviewLayer)
			{
				_zoom(recognizer.Scale);
			}
		}

		private void _zoom(nfloat scale)
		{
			try
			{
				var captureDevice = AVCaptureDevice.Devices.First();
				NSError err;
				if (captureDevice != null)
				{
					captureDevice.LockForConfiguration(out err);
					zoomScale = (nfloat)Math.Max(1.0, Math.Min(beginZoomScale * scale, maxZoomScale));
					captureDevice.VideoZoomFactor = zoomScale;
					captureDevice.UnlockForConfiguration();
				}

			}
			catch
			{
				Debug.WriteLine("error locked config");
			}
		}

		private void _updateTorch(CameraFlashMode flashMode)
		{
			if (captureSession != null)
			{
				captureSession.BeginConfiguration();

				var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
				foreach (var captureDevice in devices)
				{

					if (captureDevice.Position == AVCaptureDevicePosition.Back)
					{
						var avTorchMode = (AVCaptureTorchMode)Enum.ToObject(typeof(AVCaptureTorchMode), flashMode);

						if (captureDevice.IsTorchModeSupported(avTorchMode))
						{
							try
							{
								NSError err;
								captureDevice.LockForConfiguration(out err);
							}
							catch
							{
								return;
							}
							captureDevice.TorchMode = avTorchMode;
							captureDevice.UnlockForConfiguration();
						}
					}
				}
				captureSession.CommitConfiguration();
			}
		}

		private void _executeVideoCompletionWithURL(NSUrl url, AVCaptureVideoOrientation orientation, NSError error)
		{
			if (videoCompletion != null)
			{
				videoCompletion(url, orientation, error);
				videoCompletion = null;
			}
		}

		private AVCaptureMovieFileOutput _getMovieOutput()
		{
			var shouldReinitializeMovieOutput = (movieOutput == null);
			if (!shouldReinitializeMovieOutput)
			{
				var connection = movieOutput.ConnectionFromMediaType(AVMediaType.Video);
				if (connection != null)
				{
					shouldReinitializeMovieOutput = shouldReinitializeMovieOutput || !connection.Active;
				}
			}

			if (shouldReinitializeMovieOutput)
			{
				movieOutput = new AVCaptureMovieFileOutput();
				movieOutput.MovieFragmentInterval = CMTime.Invalid;

				captureSession.BeginConfiguration();
				captureSession.AddOutput(movieOutput);
				captureSession.CommitConfiguration();
			}
			return movieOutput;
		}

		private AVCaptureStillImageOutput _getStillImageOutput()
		{
			var shouldReinitializeStillImageOutput = (stillImageOutput == null);
			if (!shouldReinitializeStillImageOutput)
			{
				var connection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
				if (connection != null)
				{
					shouldReinitializeStillImageOutput = shouldReinitializeStillImageOutput || !connection.Active;
				}
			}
			if (shouldReinitializeStillImageOutput)
			{
				stillImageOutput = new AVCaptureStillImageOutput();

				captureSession.BeginConfiguration();
				captureSession.AddOutput(stillImageOutput);
				captureSession.CommitConfiguration();
			}
			return stillImageOutput;
		}

		private void _orientationChanged()
		{
			Debug.WriteLine("~~ ORIENTATION CHANGED ~~");
			AVCaptureConnection currentConnection;
			switch (OutputMode)
			{
				case CameraOutputMode.StillImage:
					currentConnection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
					break;
				case CameraOutputMode.VideoOnly:
				case CameraOutputMode.VideoWithMic:
				default:
					currentConnection = _getMovieOutput().ConnectionFromMediaType(AVMediaType.Video);
					break;
			}

			var validPreviewLayer = previewLayer;
			if (validPreviewLayer != null)
			{
				var validPreviewLayerConnection = validPreviewLayer.Connection;
				if (validPreviewLayerConnection != null)
				{
					if (validPreviewLayerConnection.SupportsVideoOrientation)
					{
						validPreviewLayerConnection.VideoOrientation = _currentVideoOrientation();
						Debug.WriteLine("~~ Updating PreviewLayer VideoOrientation {0} ~~", validPreviewLayerConnection.VideoOrientation);
					}
				}

				var validOutputLayerConnection = currentConnection;
				if (validOutputLayerConnection != null)
				{
					if (validOutputLayerConnection.SupportsVideoOrientation)
					{
						validOutputLayerConnection.VideoOrientation = _currentVideoOrientation();
						Debug.WriteLine("~~ Updating validOutputLayerConnection VideoOrientation {0} ~~", validOutputLayerConnection.VideoOrientation);
					}
				}

				//DispatchQueue.MainQueue.DispatchAsync(() =>
				//{
				var validEmbeddingView = this.embeddingView;
				if (validEmbeddingView != null)
				{
					validPreviewLayer.Frame = validEmbeddingView.Bounds;
					Debug.WriteLine("~~ validPreviewLayer.Frame {0},{1}", validPreviewLayer.Frame.Width, validPreviewLayer.Frame.Height);
				}
				//});
			}
		}

		private AVCaptureVideoOrientation _currentVideoOrientation()
		{
			switch (UIDevice.CurrentDevice.Orientation)
			{
				case UIDeviceOrientation.LandscapeLeft:
					return AVCaptureVideoOrientation.LandscapeRight;
				case UIDeviceOrientation.LandscapeRight:
					return AVCaptureVideoOrientation.LandscapeLeft;
				default:
					return AVCaptureVideoOrientation.Portrait;
			}
		}

		private bool _canLoadCamera()
		{
			var currentCameraState = _checkIfCameraIsAvailable();
			return currentCameraState == CameraState.Ready || (currentCameraState == CameraState.NotDetermined && showAccessPermissionPopupAutomatically);
		}

		private void _setupCamera(Action completion)
		{
			captureSession = new AVCaptureSession();

			//DispatchQueue.MainQueue.DispatchAsync(() =>
			//{
			var validCaptureSession = this.captureSession;
			if (validCaptureSession != null)
			{
				validCaptureSession.BeginConfiguration();
				validCaptureSession.SessionPreset = AVCaptureSession.PresetHigh;
				this._updateCameraDevice(this.CameraDevice);
				this._setupOutputs();
				this._setupOutputMode(this.OutputMode, null);
				this._setupPreviewLayer();
				validCaptureSession.CommitConfiguration();
				this._updateFlasMode(this.FlashMode);
				this._updateCameraQualityMode(this.CameraOutputQuality);
				validCaptureSession.StartRunning();
				this._startFollowingDeviceOrientation();
				this.cameraIsSetup = true;
				this._orientationChanged();

				completion();
			}
			//});
		}

		private void _startFollowingDeviceOrientation()
		{
			Debug.WriteLine("~~ _startFollowingDeviceOrientation ~~");
			if (ShouldRespondToOrientationChanges && !cameraIsObservingDeviceOrientation)
			{
				orientationObserver = UIDevice.Notifications.ObserveOrientationDidChange(onOrientationChanged);
				//                NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.Notifications.ObserveOrientationDidChange, _orientationChanged);
				cameraIsObservingDeviceOrientation = true;
			}
		}

		NSObject orientationObserver;

		private void _stopFollowingDeviceOrientation()
		{
			Debug.WriteLine("~~ _stopFollowingDeviceOrientation ~~");
			if (cameraIsObservingDeviceOrientation)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(orientationObserver);
				cameraIsObservingDeviceOrientation = false;
			}
		}

		void onOrientationChanged(object sender, NSNotificationEventArgs e)
		{
			if (cameraIsObservingDeviceOrientation)
			{
				Debug.WriteLine("~~ onOrientationChanged ~~");
				_orientationChanged();
			}
		}

		private void _addPreviewLayerToView(UIView view)
		{
			embeddingView = view;
			attachZoom(view);

			//DispatchQueue.MainQueue.DispatchAsync(() =>
			//{
			if (this.previewLayer == null)
				return;

			this.previewLayer.Frame = view.Layer.Bounds;
			view.ClipsToBounds = false;
			view.Layer.AddSublayer(this.previewLayer);
			//});
		}

		private void _setupMaxZoomScale()
		{
			var maxZoom = 1.0f;
			beginZoomScale = 1.0f;

			if (CameraDevice == CameraDevice.Back)
			{
				maxZoom = (float)(backCameraDevice.ActiveFormat.VideoMaxZoomFactor);
			}
			else if (CameraDevice == CameraDevice.Front)
			{
				maxZoom = (float)(frontCameraDevice.ActiveFormat.VideoMaxZoomFactor);
			}

			maxZoomScale = maxZoom;
		}

		private CameraState _checkIfCameraIsAvailable()
		{
			var deviceHasCamera = UIImagePickerController.IsCameraDeviceAvailable(UIImagePickerControllerCameraDevice.Rear)
				|| UIImagePickerController.IsCameraDeviceAvailable(UIImagePickerControllerCameraDevice.Front);
			if (deviceHasCamera)
			{
				var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);
				var userAgreedToUseIt = authorizationStatus == AVAuthorizationStatus.Authorized;
				if (userAgreedToUseIt)
				{
					return CameraState.Ready;
				}
				else if (authorizationStatus == AVAuthorizationStatus.NotDetermined)
				{
					return CameraState.NotDetermined;
				}
				else
				{
					//                            _show(NSLocalizedString("Camera access denied", comment:""), message:NSLocalizedString("You need to go to settings app and grant acces to the camera device to use it.", comment:""))
					return CameraState.AccessDenied;
				}
			}
			else
			{
				//                _show(NSLocalizedString("Camera unavailable", comment:""), message:NSLocalizedString("The device does not have a camera.", comment:""))
				return CameraState.NoDeviceFound;
			}
		}

		private void _setupOutputMode(CameraOutputMode newCameraOutputMode, CameraOutputMode? oldCameraOutputMode)
		{
			captureSession.BeginConfiguration();

			if (oldCameraOutputMode != null)
			{
				// remove current setting
				switch (oldCameraOutputMode)
				{
					case CameraOutputMode.StillImage:
						if (stillImageOutput != null)
						{
							captureSession.RemoveOutput(stillImageOutput);
						}
						break;
					case CameraOutputMode.VideoOnly:
					case CameraOutputMode.VideoWithMic:
						if (movieOutput != null)
						{
							captureSession.RemoveOutput(movieOutput);
						}
						if (oldCameraOutputMode == CameraOutputMode.VideoWithMic)
						{
							_removeMicInput();
						}
						break;
					default:
						break;
				}
			}

			// configure new devices
			switch (newCameraOutputMode)
			{
				case CameraOutputMode.StillImage:
					if (stillImageOutput == null)
					{
						_setupOutputs();
					}
					if (stillImageOutput != null)
					{
						captureSession.AddOutput(stillImageOutput);
					}
					break;
				case CameraOutputMode.VideoOnly:
				case CameraOutputMode.VideoWithMic:
					captureSession.AddOutput(_getMovieOutput());

					if (newCameraOutputMode == CameraOutputMode.VideoWithMic)
					{
						var validMic = _deviceInputFromDevice(mic);
						if (validMic != null)
						{
							captureSession.AddInput(validMic);
						}
					}
					break;
				default:
					break;
			}
			captureSession.CommitConfiguration();
			_updateCameraQualityMode(CameraOutputQuality);
			_orientationChanged();
		}

		private void _setupOutputs()
		{
			if (stillImageOutput == null)
			{
				stillImageOutput = new AVCaptureStillImageOutput();
			}
			if (movieOutput == null)
			{
				movieOutput = new AVCaptureMovieFileOutput();
				movieOutput.MovieFragmentInterval = CMTime.Invalid;
			}
			if (library == null)
			{
				library = new ALAssetsLibrary();
			}
		}

		private void _setupPreviewLayer()
		{
			if (captureSession != null)
			{
				Debug.WriteLine("_setupPreviewLayer");
				previewLayer = new AVCaptureVideoPreviewLayer(captureSession);
				previewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
			}
		}

		// TODO review, seems deviceType should be used here
		private void _updateCameraDevice(CameraDevice deviceType)
		{
			var validCaptureSession = captureSession;
			if (validCaptureSession != null)
			{
				validCaptureSession.BeginConfiguration();
				var inputs = validCaptureSession.Inputs;
				foreach (var input in inputs)
				{
					if (input != null)
					{
						var deviceInput = input as AVCaptureDeviceInput;
						if (deviceInput.Device == backCameraDevice && CameraDevice == CameraDevice.Front)
						{
							validCaptureSession.RemoveInput(deviceInput);
							break;
						}
						else if (deviceInput.Device == frontCameraDevice && CameraDevice == CameraDevice.Back)
						{
							validCaptureSession.RemoveInput(deviceInput);
							break;
						}
					}
				}

				switch (CameraDevice)
				{
					case CameraDevice.Front:
						if (hasFrontCamera)
						{
							var validFrontDevice = _deviceInputFromDevice(frontCameraDevice);
							if (validFrontDevice != null)
							{
								if (!inputs.Contains(validFrontDevice))
								{
									validCaptureSession.AddInput(validFrontDevice);
								}
							}
						}
						break;
					case CameraDevice.Back:
						var validBackDevice = _deviceInputFromDevice(backCameraDevice);
						if (validBackDevice != null)
						{
							if (!inputs.Contains(validBackDevice))
							{
								Debug.WriteLine("~~~~~ Attaching Back device ~~~~~~");
								validCaptureSession.AddInput(validBackDevice);
							}
						}
						break;
					default:
						break;
				}
				validCaptureSession.CommitConfiguration();
			}
		}

		private void _updateFlasMode(CameraFlashMode flashMode)
		{
			captureSession.BeginConfiguration();
			var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
			foreach (var device in devices)
			{

				if (device.Position == AVCaptureDevicePosition.Back)
				{
					var avFlashMode = (AVCaptureFlashMode)Enum.ToObject(typeof(AVCaptureFlashMode), flashMode);

					if (device.IsFlashModeSupported(avFlashMode))
					{
						try
						{
							NSError err;
							device.LockForConfiguration(out err);
						}
						catch
						{
							return;
						}
						device.FlashMode = avFlashMode;
						device.UnlockForConfiguration();
					}
				}
			}
			captureSession.CommitConfiguration();
		}

		private void _updateCameraQualityMode(CameraOutputQuality newCameraOutputQuality)
		{
			var validCaptureSession = captureSession;
			if (validCaptureSession != null)
			{
				var sessionPreset = AVCaptureSession.PresetLow;
				switch (newCameraOutputQuality)
				{
					case CameraOutputQuality.Low:
						sessionPreset = AVCaptureSession.PresetLow;
						break;
					case CameraOutputQuality.Medium:
						sessionPreset = AVCaptureSession.PresetMedium;
						break;
					case CameraOutputQuality.High:
						if (OutputMode == CameraOutputMode.StillImage)
						{
							sessionPreset = AVCaptureSession.PresetPhoto;
						}
						else
						{
							sessionPreset = AVCaptureSession.PresetHigh;
						}
						break;
					default:
						break;
				}

				if (validCaptureSession.CanSetSessionPreset(sessionPreset))
				{
					validCaptureSession.BeginConfiguration();
					validCaptureSession.SessionPreset = sessionPreset;
					validCaptureSession.CommitConfiguration();
				}
			}
		}

		private void _removeMicInput()
		{
			if (captureSession == null || captureSession.Inputs == null)
				return;

			var inputs = captureSession.Inputs;
			foreach (var input in inputs)
			{
				if (input != null)
				{
					var deviceInput = input as AVCaptureDeviceInput;
					if (deviceInput.Device == mic)
					{
						captureSession.RemoveInput(input);
						break;
					}
				}
			}
		}

		private void _show(string title, string message)
		{
			if (showErrorsToUsers)
			{
				Debug.WriteLine("{0},{1}", title, message);
			}
		}

		private AVCaptureDeviceInput _deviceInputFromDevice(AVCaptureDevice device = null)
		{
			if (device == null)
				return null;

			try
			{
				NSError err;
				return new AVCaptureDeviceInput(device, out err);
			}
			catch
			{
				return null;
			}
		}


		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			stopAndRemoveCaptureSession();
			_stopFollowingDeviceOrientation();
		}

		public CameraManager()
		{
		}

		public void FinishedRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections, NSError error)
		{
			this.captureOutput(captureOutput, outputFileUrl, error);
		}
	}

	public class GestureDelegate : UIGestureRecognizerDelegate
	{

	}
}

