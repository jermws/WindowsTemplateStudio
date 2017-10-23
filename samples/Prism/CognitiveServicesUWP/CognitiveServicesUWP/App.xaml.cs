using System;
using System.Globalization;
using System.Threading.Tasks;

using CognitiveServicesUWP.Services;
using CognitiveServicesUWP.Views;

using Microsoft.HockeyApp;
using Microsoft.Practices.Unity;

using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;

using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CognitiveServicesUWP.Models;

namespace CognitiveServicesUWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : PrismUnityApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        IFrameFacade frameFacade;
        ITrackingService trackingService;

        protected override INavigationService OnCreateNavigationService(IFrameFacade rootFrame)
        {
            frameFacade = rootFrame;
            frameFacade.NavigatingFrom += FrameFacade_NavigatedFrom;
            return base.OnCreateNavigationService(rootFrame);
        }

        private void FrameFacade_NavigatedFrom(object sender, NavigatingFromEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Navigated From: {e.SourcePageType.ToString()}");
            trackingService.TrackEvent($"Navigated From: {e.SourcePageType.ToString()}");
        }


        protected override void ConfigureContainer()
        {
            // Cognitive Services Computer Vision
            string apiKey = string.Empty; // TODO WTS Add Cognitive Services apiKey”

            //Image Recognition
            string baseServiceUrl = @"https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze";
            string requestParameters = "visualFeatures=Categories,Description,Color&language=en";

            //OCR Recognition
            string ocrBaseServiceUrl = @"https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr";
            string ocrRequestParameters = "language=unk&detectOrientation=true";

            //Handwriting Recognition
            string HWBaseServiceUrl = @"https://westus.api.cognitive.microsoft.com/vision/v1.0/recognizeText";
            string HWRequestParameters = "handwriting=true";


            base.ConfigureContainer();
            Container.RegisterType<ITrackingService, TrackingService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICognitiveServicesVisionRepository<ImageResponse>, CognitiveServicesVisionRepository<ImageResponse>>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICognitiveServicesVisionRepository<TextImageResponse>, CognitiveServicesVisionRepository<TextImageResponse>>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICognitiveServicesVisionRepository<HandWritingImageResponse>, CognitiveServicesVisionRepository<HandWritingImageResponse>>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICognitiveServicesVisionServiceProxy<ImageResponse>, CognitiveServicesVisionServiceProxy<ImageResponse>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(baseServiceUrl, apiKey, requestParameters));
            Container.RegisterType<ICognitiveServicesVisionServiceProxy<TextImageResponse>, CognitiveServicesVisionServiceProxy<TextImageResponse>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(ocrBaseServiceUrl, apiKey, ocrRequestParameters));
            Container.RegisterType<ICognitiveServicesVisionServiceProxy<HandWritingImageResponse>, CognitiveServicesVisionServiceProxy<HandWritingImageResponse>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(HWBaseServiceUrl, apiKey, HWRequestParameters));
            Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));
        }

        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            return LaunchApplication(PageTokens.MainPage, null);
        }

        private async Task LaunchApplication(string page, object launchParam)
        {
            NavigationService.Navigate(page, launchParam);
            Window.Current.Activate();
            await Task.CompletedTask;
        }

        protected override Task OnActivateApplicationAsync(IActivatedEventArgs args)
        {
            return Task.CompletedTask;
        }

        protected async override Task OnInitializeAsync(IActivatedEventArgs args)
        {
           string HockeyAppId = "5d560c2f7d9a40de91cb29ad89e441c2"; //string.Empty;
            if (!string.IsNullOrEmpty(HockeyAppId))
            {

                TelemetryConfiguration telemetryConfig = new TelemetryConfiguration();
                telemetryConfig.EnableDiagnostics = true;
                telemetryConfig.Collectors = WindowsCollectors.Metadata | WindowsCollectors.Session | WindowsCollectors.UnhandledException;

                HockeyClient.Current.Configure(HockeyAppId, telemetryConfig);
            }
            trackingService = Current.Container.Resolve<ITrackingService>();
            // We are remapping the default ViewNamePage->ViewNamePageViewModel naming to ViewNamePage->ViewNameViewModel to 
            // gain better code reuse with other frameworks and pages within Windows Template Studio
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewModelTypeName = string.Format(CultureInfo.InvariantCulture, "CognitiveServicesUWP.ViewModels.{0}ViewModel, CognitiveServicesUWP", viewType.Name.Substring(0,viewType.Name.Length - 4));
                return Type.GetType(viewModelTypeName);
            });
            await base.OnInitializeAsync(args);
        }

        public void SetNavigationFrame(Frame frame)
        {
            var sessionStateService = Container.Resolve<ISessionStateService>();
            base.CreateNavigationService(new FrameFacadeAdapter(frame), sessionStateService);
        }

        protected override UIElement CreateShell(Frame rootFrame)
        {
            var shell = Container.Resolve<ShellPage>();
            shell.SetRootFrame(rootFrame);
            return shell;
        }
    }
}
