using Param_RootNamespace.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Param_RootNamespace.Services
{
    public interface IComputerVisionServiceProxy
    {
        Task<ImageResponse> GetImageResponse(byte [] imageArray);
    }
}
