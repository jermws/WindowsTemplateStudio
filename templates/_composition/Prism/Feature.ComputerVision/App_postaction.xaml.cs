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
            Container.RegisterType<IComputerVisionRepository, ComputerVisionRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IComputerVisionServiceProxy, ComputerVisionServiceProxy>(new ContainerControlledLifetimeManager());
//}]}
        }
    }
}