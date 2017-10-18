using Windows.UI.Xaml;
//^^
//{[{
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
            Container.RegisterType<ICognitiveServicesVisionRepository, CognitiveServicesVisionRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICognitiveServicesVisionServiceProxy, CognitiveServicesVisionServiceProxy>(new ContainerControlledLifetimeManager());
//}]}
        }
    }
}