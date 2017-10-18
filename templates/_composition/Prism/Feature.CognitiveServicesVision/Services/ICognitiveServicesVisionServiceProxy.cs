﻿using System.Threading.Tasks;
using Param_RootNamespace.Models;

namespace Param_RootNamespace.Services
{
    public interface ICognitiveServicesVisionServiceProxy
    {
        Task<ImageResponse> GetImageResponse(byte [] imageArray);
    }
}
