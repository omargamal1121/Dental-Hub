using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;

namespace DentalHub.Application.Services.Doctors
{
    public interface IDoctorService
    {
        // Doctor Profile
        Task<Result<DoctorDto>> GetDoctorByIdAsync(Guid userId);
        Task<Result<PagedResult<DoctorlistDto>>> GetAllDoctorsAsync(int page = 1, int pageSize = 10, string? name = null, string? spec = null);
        Task<Result<DoctorDto>> UpdateDoctorAsync(UpdateDoctorDto dto);
        Task<Result> DeleteDoctorAsync(Guid userId);

        // Doctor Statistics
        Task<Result<DoctorStatsDto>> GetDoctorStatisticsAsync(Guid doctorId);

        // Get doctors by university
        Task<Result<PagedResult<DoctorDto>>> GetDoctorsByUniversityAsync(
            string universityId, int page = 1, int pageSize = 10);
    }
}
