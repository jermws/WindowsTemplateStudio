using System;

using CognitiveServicesUWP.ViewModels;

using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CognitiveServicesUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OCRPage : Page
    {
        private OCRViewModel ViewModel => DataContext as OCRViewModel;
        public OCRPage()
        {
            this.InitializeComponent();
        }
    }
}
