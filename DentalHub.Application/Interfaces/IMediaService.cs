using Microsoft.AspNetCore.Http;

namespace DentalHub.Application.Interfaces
{
    public interface IMediaService
    {
        Task<ImageUploadDto> UploadImageAsync(IFormFile file);
        Task<bool> DeleteImageAsync(string publicId);



    }
}
