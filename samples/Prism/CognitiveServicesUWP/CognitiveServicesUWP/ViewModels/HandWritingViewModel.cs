using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using CognitiveServicesUWP.Models;
using CognitiveServicesUWP.Services;
using CognitiveServicesUWP.Views;

using Prism.Commands;
using Prism.Windows.Mvvm;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace CognitiveServicesUWP.ViewModels
{
    public class HandWritingViewModel : ViewModelBase
    {
        /// <summary>
        /// The _contact repository
        /// </summary>
        private readonly ICognitiveServicesVisionRepository<HandWritingImageResponse> cognitiveServicesVisionRepository;

        private HandWritingImageResponse _imageResponse;
        private byte[] _imageArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="ICognitiveServicesVisionRepository" /> class.
        /// </summary>
        /// <param name="cognitiveServicesVisionRepository">The computer vision repository.</param>
        public HandWritingViewModel(ICognitiveServicesVisionRepository<HandWritingImageResponse> cognitiveServicesVisionRepository)
        {
            this.cognitiveServicesVisionRepository = cognitiveServicesVisionRepository;

            TakePhotoCommand = new DelegateCommand(TakePhoto);
            PickPhotoCommand = new DelegateCommand(PickPhoto);

            IsBusy = false;
        }

        public DelegateCommand TakePhotoCommand { get; set; }
        public DelegateCommand PickPhotoCommand { get; set; }

        private async void TakePhoto()
        {
            var dialog = new CameraCaptureDialog();
            await dialog.ShowAsync();
            if (dialog.ImageBytes != null)
            {
                ClearResponse();
                IRandomAccessStream fileStream = dialog.ImageBytes.AsBuffer().AsStream().AsRandomAccessStream();
                // Set the image source to the selected bitmap 
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.DecodePixelWidth = 600; //match the target Image.Width, not shown
                await bitmapImage.SetSourceAsync(fileStream);
                PhotoUrl = bitmapImage;
                _imageArray = dialog.ImageBytes;
                await ComputerVisionAsync();
            }
        }

        private async void PickPhoto()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            // Open a stream for the selected file 
            StorageFile file = await openPicker.PickSingleFileAsync();
            // Ensure a file was selected 
            if (file != null)
            {
                ClearResponse();
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap 
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.DecodePixelWidth = 600; //match the target Image.Width, not shown
                    await bitmapImage.SetSourceAsync(fileStream);
                    PhotoUrl = bitmapImage;
                }
                using (var memoryStream = new MemoryStream())
                {
                    var stream = await file.OpenStreamForReadAsync();
                    _imageArray = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(_imageArray, 0, (int)stream.Length);
                }
                await ComputerVisionAsync();
            }
        }

        private async Task ComputerVisionAsync()
        {
            ComputerVision1Results = string.Empty;
            IsBusy = true;

            // validate if there is an image
            if (photoUrl != null && _imageArray != null && _imageArray.Length > 0)
            {
                _imageResponse = await cognitiveServicesVisionRepository.GetImageResponseAsync(_imageArray);
                FormatResponse(_imageResponse);
                IsBusy = false;
            }
        }
        private void ClearResponse()
        {
            Captions = string.Empty;
        }

        private void FormatResponse(HandWritingImageResponse imageResponse)
        {
            StringBuilder sbText = new StringBuilder();


            if (imageResponse != null && imageResponse.recognitionResult != null)
            {
                foreach (var lines in imageResponse.recognitionResult.lines)
                {
                    foreach (var word in lines.words)
                    {
                        sbText.Append(word.text + " ");
                    }
                    sbText.AppendLine();
                }
            }
            Captions = sbText.ToString();
        }

        private ImageSource photoUrl;
        public ImageSource PhotoUrl
        {
            get
            {
                return photoUrl;
            }

            set
            {
                SetProperty(ref photoUrl, value);
            }
        }

        private string _computerVisionResults;
        public string ComputerVision1Results
        {
            get
            {
                return _computerVisionResults;
            }
            set
            {
                SetProperty(ref _computerVisionResults, value);
            }
        }

        private string _captions;
        public string Captions
        {
            get
            {
                return _captions;
            }
            set
            {
                SetProperty(ref _captions, value);
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                SetProperty(ref _isBusy, value);
                RaisePropertyChanged(nameof(IsBusy));
            }
        }
    }
}
