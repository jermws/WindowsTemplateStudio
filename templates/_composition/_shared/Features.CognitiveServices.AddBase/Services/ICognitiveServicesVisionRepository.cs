using System.Threading.Tasks;

using Param_RootNamespace.Models;

namespace Param_RootNamespace.Services
{
    public interface ICognitiveServicesVisionRepository<T>
    {
        Task<T> GetImageResponseAsync(byte[] imageArray);
    }
}
