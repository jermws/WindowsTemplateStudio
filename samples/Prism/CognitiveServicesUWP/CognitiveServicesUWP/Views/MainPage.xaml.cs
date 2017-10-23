using System;

using CognitiveServicesUWP.ViewModels;

using Windows.UI.Xaml.Controls;

namespace CognitiveServicesUWP.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel => DataContext as MainViewModel; 
        public MainPage()
        {
            InitializeComponent();
        }
    }
}
