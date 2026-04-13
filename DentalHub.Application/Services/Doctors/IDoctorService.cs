using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;

namespace DentalHub.Application.Services.Doctors
{
    public interface IDoctorService
    {
        // Doctor Profile - PublicId for Admin
        Task<Result<DoctorDto>> GetDoctorByIdAsync(Guid id);

        // Doctor Profile - Token
        Task<Result<DoctorDto>> GetDoctorByUserIdAsync(Guid userId);

        Task<Result<PagedResult<DoctorlistDto>>> GetAllDoctorsAsync(
            int page = 1, int pageSize = 10, string? name = null, string? spec = null, string? username = null, Guid? universityId = null);

        Task<Result<DoctorDto>> UpdateDoctorAsync(UpdateDoctorDto dto);

        Task<Result> DeleteDoctorAsync(Guid id);

        // Doctor Statistics - Token
        Task<Result<DoctorStatsDto>> GetDoctorStatisticsAsync(Guid userId);

        // Get doctors by university
        Task<Result<PagedResult<DoctorDto>>> GetDoctorsByUniversityAsync(
             Guid universityId, int page = 1, int pageSize = 10);
        Task<Result> HandleBeforeDeleteAsync(Guid id);

        Task<Result<List<DoctorLookupDto>>> GetDoctorsByUniversityAsync(Guid universityId);
    }
}
