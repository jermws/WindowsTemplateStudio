using Param_RootNamespace.Models;
using Param_RootNamespace.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Param_RootNamespace.Services
{
     public class ComputerVisionRepository : IComputerVisionRepository
    {
        private IComputerVisionServiceProxy computerVisionServiceProxy;
        private ImageResponse imageResponse;

        public ComputerVisionRepository(IComputerVisionServiceProxy computerVisionServiceProxy)
        {
            this.computerVisionServiceProxy = computerVisionServiceProxy;
        }
        public async Task<ImageResponse> GetImageResponseAsync(byte[] imageArray)
        {
            var imageResponse = await computerVisionServiceProxy.GetImageResponse(imageArray);
            return imageResponse;
        }
    }
}
