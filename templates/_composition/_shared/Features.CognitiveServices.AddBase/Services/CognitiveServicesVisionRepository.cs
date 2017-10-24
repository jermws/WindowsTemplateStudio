using System.Threading.Tasks;
using  Param_RootNamespace.Models;

namespace  Param_RootNamespace.Services
{
    public class CognitiveServicesVisionRepository<T> : ICognitiveServicesVisionRepository<T>
    {
        private ICognitiveServicesVisionServiceProxy<T> cognitiveServicesVisionProxy;
        public CognitiveServicesVisionRepository(ICognitiveServicesVisionServiceProxy<T> cognitiveServicesVisionServiceProxy)
        {
            this.cognitiveServicesVisionProxy = cognitiveServicesVisionServiceProxy;
        }
        public async Task<T> GetImageResponseAsync(byte[] imageArray)
        {
            var imageResponse = await cognitiveServicesVisionProxy.GetImageResponseAsync(imageArray);
            return imageResponse;
        }
    }
    
}
