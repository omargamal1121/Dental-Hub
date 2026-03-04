using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Patients;
using static DentalHub.Application.Services.PatientService;

namespace DentalHub.Application.Services
{
    public interface IPatientService
    {
        Task<Result<PatientDto>> GetPatientByIdAsync(Guid id);

        // For the patient himself - searches by UserId (coming from the JWT token)
        Task<Result<PatientDto>> GetPatientByUserIdAsync(Guid userId);

        Task<Result<PagedResult<PatientDto>>> GetAllPatientsAsync(FilterPatientDto filterPatientDto, int page = 1, int pageSize = 10);


        Task<Result<PatientDto>> UpdatePatientAsync(UpdatePatientDto dto);
        Task<Result> DeletePatientAsync(Guid id);
    }
}
