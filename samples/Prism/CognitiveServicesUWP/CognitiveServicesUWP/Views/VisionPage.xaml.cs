using System;

using CognitiveServicesUWP.ViewModels;

using Windows.UI.Xaml.Controls;

namespace CognitiveServicesUWP.Views
{
    public sealed partial class VisionPage : Page
    {
        private VisionViewModel ViewModel => DataContext as VisionViewModel; 
        public VisionPage()
        {
            this.InitializeComponent();
        }
    }
}
