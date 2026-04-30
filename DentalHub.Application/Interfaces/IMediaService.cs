using DentalHub.Application.Common;
using DentalHub.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DentalHub.Application.Interfaces
{
    public interface IMediaService
    {
        Task<Result<ImageUploadDto>> UploadImageAsync(IFormFile file);
        Task<bool> DeleteImageAsync(string publicId);

        Task<Result<Media>> SavePatientMediaAsync(IFormFile file, Guid patientId);
        Task<Result<Media>> SaveSessionMediaAsync(IFormFile file, Guid sessionId);
        Task<Result<Media>> SaveNoteMediaAsync(IFormFile file, Guid noteId);
        Task<Result<Media>> SaveCaseTypeMediaAsync(IFormFile file, Guid caseTypeId);
        Task<Result<Media>> SaveCaseMediaAsync(IFormFile file, Guid caseId);



    }
}
