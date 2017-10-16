using Windows.UI.Xaml;
//^^
//{[{
using Param_RootNamespace.Services;
using Microsoft.HockeyApp;
//}]}

namespace Param_RootNamespace
{
    public sealed partial class App : PrismUnityApplication
    {
        public App()
        {
            InitializeComponent();
        }

//{[{
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
//}]}

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
//{[{
            Container.RegisterType<ITrackingService, TrackingService>(new ContainerControlledLifetimeManager());
//}]}
        }

        private async Task LaunchApplication(string page, object launchParam)
        {
            NavigationService.Navigate(page, launchParam);            
            Window.Current.Activate();
            await Task.CompletedTask;
        }

        protected async override Task OnInitializeAsync(IActivatedEventArgs args)
        {
//{[{
           string HockeyAppId = "5d560c2f7d9a40de91cb29ad89e441c2"; //string.Empty;
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                HockeyAppId = string.Empty; // disable sending telemetry to HockeyApp/AppInsights
                //this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            if (!string.IsNullOrEmpty(HockeyAppId))
            {

                TelemetryConfiguration telemetryConfig = new TelemetryConfiguration();
                telemetryConfig.EnableDiagnostics = true;
                telemetryConfig.Collectors = WindowsCollectors.Metadata | WindowsCollectors.Session | WindowsCollectors.UnhandledException;

                HockeyClient.Current.Configure(HockeyAppId, telemetryConfig);
            }
            trackingService = Current.Container.Resolve<ITrackingService>();
//}]}
            await base.OnInitializeAsync(args);
        }
    }
}
