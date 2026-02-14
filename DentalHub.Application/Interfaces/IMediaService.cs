using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Interfaces
{
    public interface IMediaService
    {
        Task<ImageUploadDto> UploadImageAsync(IFormFile file);
        Task<bool> DeleteImageAsync(string publicId);



    }
}
