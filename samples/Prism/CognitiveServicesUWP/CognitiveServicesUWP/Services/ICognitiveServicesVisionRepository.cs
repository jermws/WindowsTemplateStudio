using System.Threading.Tasks;

using CognitiveServicesUWP.Models;

namespace CognitiveServicesUWP.Services
{
    public interface ICognitiveServicesVisionRepository
    {
        Task<ImageResponse> GetImageResponseAsync(byte [] imageArray);
    }


    public interface ICognitiveServicesVisionRepository<T>
    {
        Task<T> GetImageResponseAsync(byte[] imageArray);
    }
}
