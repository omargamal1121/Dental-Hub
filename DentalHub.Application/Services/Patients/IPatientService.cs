using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Patients;
using static DentalHub.Application.Services.PatientService;

namespace DentalHub.Application.Services
{
    public interface IPatientService
    {
        Task<Result<PatientDto>> GetPatientByPublicIdAsync(string publicId);

        // For the patient himself - searches by UserId (coming from the JWT token)
        Task<Result<PatientDto>> GetPatientByUserIdAsync(string userId);

        Task<Result<PagedResult<PatientDto>>> GetAllPatientsAsync(FilterPatientDto filterPatientDto, int page = 1, int pageSize = 10);


        Task<Result<PatientDto>> UpdatePatientAsync(UpdatePatientDto dto);
        Task<Result> DeletePatientAsync(string publicId);
    }
}
