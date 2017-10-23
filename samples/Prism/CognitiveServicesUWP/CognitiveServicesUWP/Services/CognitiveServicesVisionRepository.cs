using System.Threading.Tasks;

using CognitiveServicesUWP.Models;

namespace CognitiveServicesUWP.Services
{
    //public class CognitiveServicesVisionRepository : ICognitiveServicesVisionRepository
    //{
    //    //private ICognitiveServicesVisionServiceProxy  cognitiveServicesVisionProxy;
    //    private ICognitiveServicesVisionServiceProxy<ImageResponse> cognitiveServicesVisionProxy;
    //    public CognitiveServicesVisionRepository(ICognitiveServicesVisionServiceProxy<ImageResponse> cognitiveServicesVisionServiceProxy)
    //    {
    //        this.cognitiveServicesVisionProxy = cognitiveServicesVisionServiceProxy;
    //    }
    //    public async Task<ImageResponse> GetImageResponseAsync(byte[] imageArray)
    //    {
    //        var imageResponse = await cognitiveServicesVisionProxy.GetImageResponseAsync(imageArray);
    //        return imageResponse;
    //    }
    //}


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
