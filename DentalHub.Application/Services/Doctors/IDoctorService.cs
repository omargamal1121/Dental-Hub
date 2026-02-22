using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;

namespace DentalHub.Application.Services.Doctors
{
    public interface IDoctorService
    {
        // Doctor Profile - PublicId for Admin
        Task<Result<DoctorDto>> GetDoctorByPublicIdAsync(string publicId);

        // Doctor Profile - Token
        Task<Result<DoctorDto>> GetDoctorByIdAsync(string userId);

        Task<Result<PagedResult<DoctorlistDto>>> GetAllDoctorsAsync(
            int page = 1, int pageSize = 10, string? name = null, string? spec = null);

        Task<Result<DoctorDto>> UpdateDoctorAsync(UpdateDoctorDto dto);

        Task<Result> DeleteDoctorAsync(string publicId);

        // Doctor Statistics - Token
        Task<Result<DoctorStatsDto>> GetDoctorStatisticsAsync(string userId);

        // Get doctors by university
        Task<Result<PagedResult<DoctorDto>>> GetDoctorsByUniversityAsync(
            string universityId, int page = 1, int pageSize = 10);
    }
}
