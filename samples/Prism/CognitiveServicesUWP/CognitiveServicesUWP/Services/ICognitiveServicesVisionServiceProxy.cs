using System.Threading.Tasks;

using CognitiveServicesUWP.Models;

namespace CognitiveServicesUWP.Services
{
    //public interface ICognitiveServicesVisionServiceProxy
    //{
    //    Task<ImageResponse> GetImageResponse(byte [] imageArray);
    //}


    public interface ICognitiveServicesVisionServiceProxy<T>
    {
        Task<T> GetImageResponseAsync(byte[] imageArray);
    }
}
