using Windows.UI.Xaml;
//^^
//{[{
using Param_RootNamespace.Models;
using Param_RootNamespace.Services;
//}]}

namespace Param_RootNamespace
{
    public sealed partial class App : PrismUnityApplication
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
//{[{
            //Image Recognition
            string baseServiceUrl = @"https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze";
            string requestParameters = "visualFeatures=Categories,Description,Color&language=en";

            Container.RegisterType<ICognitiveServicesVisionRepository<ImageResponse>, CognitiveServicesVisionRepository<ImageResponse>>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICognitiveServicesVisionServiceProxy<ImageResponse>, CognitiveServicesVisionServiceProxy<ImageResponse>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(baseServiceUrl, apiKey, requestParameters));
//}]}
        }
    }
}