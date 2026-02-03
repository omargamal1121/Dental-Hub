using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Patients;

namespace DentalHub.Application.Services.Patients
{
    public interface IPatientService
    {
        Task<Result<PatientDto>> GetPatientByIdAsync(Guid userId);
        Task<Result<List<PatientDto>>> GetAllPatientsAsync(int page = 1, int pageSize = 10);
        Task<Result<PatientDto>> UpdatePatientAsync(UpdatePatientDto dto);
        Task<Result> DeletePatientAsync(Guid userId);
    }
}
