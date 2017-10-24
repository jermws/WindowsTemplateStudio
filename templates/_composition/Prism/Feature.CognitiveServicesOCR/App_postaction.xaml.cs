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
//^^
//{[{
            //OCR Recognition
            string ocrBaseServiceUrl = @"https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr";
            string ocrRequestParameters = "language=unk&detectOrientation=true";

            Container.RegisterType<ICognitiveServicesVisionRepository<TextImageResponse>, CognitiveServicesVisionRepository<TextImageResponse>>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICognitiveServicesVisionServiceProxy<TextImageResponse>, CognitiveServicesVisionServiceProxy<TextImageResponse>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(ocrBaseServiceUrl, apiKey, ocrRequestParameters));
//}]}
        }
    }
}