using Param_ItemNamespace.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Param_ItemNamespace.Views
{
    public sealed partial class CameraCaptureDialog : ContentDialog
    {

        public byte[] ImageBytes
        {
            get;
            set;
        }

        // Rotation metadata to apply to the preview stream and recorded videos (MF_MT_VIDEO_ROTATION)
        // Reference: http://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh868174.aspx
        private static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");

        private MediaCapture _mediaCapture;
        private bool _isInitialized;
        private bool _isPreviewing;
        private bool _isRecording;

        // Information about the camera device
        private bool _mirroringPreview;
        private bool _externalCamera;

        // Rotation Helper to simplify handling rotation compensation for the camera streams
        private CameraRotationHelper _rotationHelper;

        private Task _setupTask = Task.CompletedTask;

        // Prevent the screen from sleeping while the camera is running
        private readonly DisplayRequest _displayRequest = new DisplayRequest();

        public CameraCaptureDialog()
        {
            this.InitializeComponent();
            this.Loaded += CameraCaptureDialog_Loaded;
        }

        private async void CameraCaptureDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ImageBytes = null;
            await InitializeCameraAsync();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            await TakePhotoAsync();
            await CleanupCameraAsync();
        }

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            await CleanupCameraAsync();
        }

        /// <summary>
        /// Initializes the MediaCapture, registers events, gets camera device information for mirroring and rotating, starts preview and unlocks the UI
        /// </summary>
        /// <returns></returns>
        private async Task InitializeCameraAsync()
        {
            Debug.WriteLine("InitializeCameraAsync");

            if (_mediaCapture == null)
            {
                // Attempt to get the back camera if one is available, but use any camera device if not
                var cameraDevice = await FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel.Back);

                if (cameraDevice == null)
                {
                    Debug.WriteLine("No camera device found!");
                    return;
                }

                // Create MediaCapture and its settings
                _mediaCapture = new MediaCapture();

                // Register for a notification when video recording has reached the maximum time and when something goes wrong
                _mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;
                _mediaCapture.Failed += MediaCapture_Failed;

                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

                // Initialize MediaCapture
                try
                {
                    await _mediaCapture.InitializeAsync(settings);
                    _isInitialized = true;
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("The app was denied access to the camera");
                }

                // If initialization succeeded, start the preview
                if (_isInitialized)
                {
                    // Figure out where the camera is located
                    if (cameraDevice.EnclosureLocation == null || cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
                    {
                        // No information on the location of the camera, assume it's an external camera, not integrated on the device
                        _externalCamera = true;
                    }
                    else
                    {
                        // Camera is fixed on the device
                        _externalCamera = false;

                        // Only mirror the preview if the camera is on the front panel
                        _mirroringPreview = (cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
                    }

                    // Initialize rotationHelper
                    _rotationHelper = new CameraRotationHelper(cameraDevice.EnclosureLocation);
                    _rotationHelper.OrientationChanged += RotationHelper_OrientationChanged;

                    await StartPreviewAsync();
                }
            }
        }

        /// <summary>
        /// Starts the preview and adjusts it for for rotation and mirroring after making a request to keep the screen on
        /// </summary>
        /// <returns></returns>
        private async Task StartPreviewAsync()
        {
            // Prevent the device from sleeping while the preview is running
            _displayRequest.RequestActive();

            // Set the preview source in the UI and mirror it if necessary
            PreviewControl.Source = _mediaCapture;
            PreviewControl.FlowDirection = _mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            // Start the preview
            await _mediaCapture.StartPreviewAsync();
            _isPreviewing = true;

            // Initialize the preview to the current orientation
            if (_isPreviewing)
            {
                await SetPreviewRotationAsync();
            }
        }

        private async void MediaCapture_RecordLimitationExceeded(MediaCapture sender)
        {
            // This is a notification that recording has to stop, and the app is expected to finalize the recording
            await StopRecordingAsync();
        }

        /// <summary>
        /// Stops recording a video
        /// </summary>
        /// <returns></returns>
        private async Task StopRecordingAsync()
        {
            Debug.WriteLine("Stopping recording...");

            _isRecording = false;
            await _mediaCapture.StopRecordAsync();

            Debug.WriteLine("Stopped recording!");
        }


        private async void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            Debug.WriteLine("MediaCapture_Failed: (0x{0:X}) {1}", errorEventArgs.Code, errorEventArgs.Message);

            await CleanupCameraAsync();
        }

        private async void RotationHelper_OrientationChanged(object sender, bool updatePreview)
        {
            if (updatePreview)
            {
                await SetPreviewRotationAsync();
            }
        }

        /// <summary>
        /// Gets the current orientation of the UI in relation to the device (when AutoRotationPreferences cannot be honored) and applies a corrective rotation to the preview
        /// </summary>
        private async Task SetPreviewRotationAsync()
        {
            // Only need to update the orientation if the camera is mounted on the device
            if (_externalCamera) return;

            // Add rotation metadata to the preview stream to make sure the aspect ratio / dimensions match when rendering and getting preview frames
            var rotation = _rotationHelper.GetCameraPreviewOrientation();
            var props = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            props.Properties.Add(RotationKey, CameraRotationHelper.ConvertSimpleOrientationToClockwiseDegrees(rotation));
            await _mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);
        }

        /// <summary>
        /// Cleans up the camera resources (after stopping any video recording and/or preview if necessary) and unregisters from MediaCapture events
        /// </summary>
        /// <returns></returns>
        private async Task CleanupCameraAsync()
        {
            Debug.WriteLine("CleanupCameraAsync");

            if (_isInitialized)
            {
                // If a recording is in progress during cleanup, stop it to save the recording
                if (_isRecording)
                {
                    await StopRecordingAsync();
                }

                if (_isPreviewing)
                {
                    // The call to stop the preview is included here for completeness, but can be
                    // safely removed if a call to MediaCapture.Dispose() is being made later,
                    // as the preview will be automatically stopped at that point
                    await StopPreviewAsync();
                }

                _isInitialized = false;
            }

            if (_mediaCapture != null)
            {
                _mediaCapture.RecordLimitationExceeded -= MediaCapture_RecordLimitationExceeded;
                _mediaCapture.Failed -= MediaCapture_Failed;
                _mediaCapture.Dispose();
                _mediaCapture = null;
            }

            if (_rotationHelper != null)
            {
                _rotationHelper.OrientationChanged -= RotationHelper_OrientationChanged;
                _rotationHelper = null;
            }
        }

        /// <summary>
        /// Stops the preview and deactivates a display request, to allow the screen to go into power saving modes
        /// </summary>
        /// <returns></returns>
        private async Task StopPreviewAsync()
        {
            // Stop the preview
            _isPreviewing = false;
            await _mediaCapture.StopPreviewAsync();

            // Use the dispatcher because this method is sometimes called from non-UI threads
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Cleanup the UI
                PreviewControl.Source = null;

                // Allow the device screen to sleep now that the preview is stopped
                _displayRequest.RequestRelease();
            });
        }

        /// <summary>
        /// Attempts to find and return a device mounted on the panel specified, and on failure to find one it will return the first device listed
        /// </summary>
        /// <param name="desiredPanel">The desired panel on which the returned device should be mounted, if available</param>
        /// <returns></returns>
        private static async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel desiredPanel)
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the desired camera by panel
            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);

            // If there is no device mounted on the desired panel, return the first device found
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }


        /// <summary>
        /// Takes a photo to a StorageFile and adds rotation metadata to it
        /// </summary>
        /// <returns></returns>
        private async Task TakePhotoAsync()
        {
            try
            {
                var stream = new InMemoryRandomAccessStream();

                Debug.WriteLine("Taking photo...");
                await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

                using (var memoryStream = new MemoryStream())
                {
                    ImageBytes = new byte[stream.Size];
                    stream.Seek(0);
                    var bytes = new byte[stream.Size];

                    // This returns IAsyncOperationWithProgess, so you can add additional progress handling
                    await stream.ReadAsync(bytes.AsBuffer(), (uint)stream.Size, Windows.Storage.Streams.InputStreamOptions.None);
                    ImageBytes = bytes;
                }
            }
            catch (Exception ex)
            {
                // File I/O errors are reported as exceptions
                Debug.WriteLine("Exception when taking a photo: " + ex.ToString());
            }
        }
    }
}
