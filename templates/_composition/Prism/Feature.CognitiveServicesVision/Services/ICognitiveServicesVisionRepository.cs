using System.Threading.Tasks;
using Param_RootNamespace.Models;

namespace Param_RootNamespace.Services
{
    public interface ICognitiveServicesVisionRepository
    {
        Task<ImageResponse> GetImageResponseAsync(byte [] imageArray);
    }
}
