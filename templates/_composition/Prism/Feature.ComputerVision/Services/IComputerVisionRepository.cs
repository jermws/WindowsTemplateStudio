
using Param_RootNamespace.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Param_RootNamespace.Services
{
    internal interface IComputerVisionRepository
    {
        Task<ImageResponse> GetImageResponseAsync(byte [] imageArrag);
    }
}
