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
    public class VisionViewModel : ViewModelBase
    {
        /// <summary>
        /// The _contact repository
        /// </summary>
        private readonly ICognitiveServicesVisionRepository<ImageResponse> cognitiveServicesVisionRepository;

        private ImageResponse _imageResponse;
        private byte[] _imageArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="ICognitiveServicesVisionRepository" /> class.
        /// </summary>
        /// <param name="cognitiveServicesVisionRepository">The computer vision repository.</param>
        public VisionViewModel(ICognitiveServicesVisionRepository<ImageResponse> cognitiveServicesVisionRepository)
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
            Tags = string.Empty;
            ImageInfo = string.Empty;
            Colors = string.Empty;
            Categories = string.Empty;
        }

        private void FormatResponse(ImageResponse imageResponse)
        {

            StringBuilder captions = new StringBuilder();
            StringBuilder tags = new StringBuilder();
            string imageInfo = string.Empty;
            string colors = string.Empty;
            StringBuilder categories = new StringBuilder();

            if (imageResponse.description != null)
            {
                if (imageResponse.description.captions != null)
                {
                    for (int i = 0; i < imageResponse.description.captions.Length; i++)
                    {
                        captions.AppendFormat((i != imageResponse.description.captions.Length - 1) ? "{0} {1:F2}% Confidence \n" : "{0} {1:F2}% confidence", imageResponse.description.captions[i].text, imageResponse.description.captions[i].confidence * 100);
                    }
                }

                if (imageResponse.description.tags != null)
                {
                    for (int j = 0; j < imageResponse.description.tags.Length; j++)
                    {
                        tags.AppendFormat(j != imageResponse.description.tags.Length - 1 ? "{0}, " : "{0}", imageResponse.description.tags[j]);
                    }
                }

                if (imageResponse.metadata != null)
                {
                    imageInfo = String.Format("{0}x{1} {2}", imageResponse.metadata.width, imageResponse.metadata.height, imageResponse.metadata.format);
                }

                if (imageResponse.color != null)
                {
                    colors = String.Format("Background: {0} Foreground: {1} Accent: {2}", imageResponse.color.dominantColorBackground, imageResponse.color.dominantColorForeground, imageResponse.color.accentColor);
                }

                if (imageResponse.categories != null)
                {
                    for (int k = 0; k < imageResponse.categories.Length; k++)
                    {
                        categories.AppendFormat(k != imageResponse.categories.Length - 1 ? "{0} {1:F2}% Confidence \n" : "{0} {1:F2}% Confidence", imageResponse.categories[k].name, imageResponse.categories[k].score * 100);
                    }
                }
            }

            if (captions.Length > 0)
                Captions = captions.ToString();
            if (tags.Length > 0)
                Tags = tags.ToString();
            if (!String.IsNullOrEmpty(imageInfo))
                ImageInfo = imageInfo;
            if (!String.IsNullOrEmpty(colors))
                Colors = colors;
            if (categories.Length > 0)
                Categories = categories.ToString();
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

        private string _tags;
        public string Tags
        {
            get
            {
                return _tags;
            }
            set
            {
                SetProperty(ref _tags, value);
            }
        }

        private string _colors;
        public string Colors
        {
            get
            {
                return _colors;
            }
            set
            {
                SetProperty(ref _colors, value);
            }
        }

        private string _imageInfo;
        public string ImageInfo
        {
            get
            {
                return _imageInfo;
            }
            set
            {
                SetProperty(ref _imageInfo, value);
            }
        }

        private string _categories;
        public string Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                SetProperty(ref _categories, value);
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
