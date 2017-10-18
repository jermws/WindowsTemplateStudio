using Param_RootNamespace.Models;
using System.Threading.Tasks;

namespace Param_RootNamespace.Services
{
    public class CognitiveServicesVisionRepository : ICognitiveServicesVisionRepository
    {
        private ICognitiveServicesVisionServiceProxy cognitiveServicesVisionProxy;
        private ImageResponse imageResponse;

        public CognitiveServicesVisionRepository(ICognitiveServicesVisionServiceProxy cognitiveServicesVisionServiceProxy)
        {
            this.cognitiveServicesVisionProxy = cognitiveServicesVisionServiceProxy;
        }
        public async Task<ImageResponse> GetImageResponseAsync(byte[] imageArray)
        {
            var imageResponse = await cognitiveServicesVisionProxy.GetImageResponse(imageArray);
            return imageResponse;
        }
    }
}
