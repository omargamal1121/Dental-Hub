using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Diagnoses;

namespace DentalHub.Application.Services.DiagnosesServices
{
    public interface IDiagnosisService
    {
        Task<Result<DiagnosisDto>> GetDiagnosisByIdAsync(Guid id);
        Task<Result<PagedResult<DiagnosisDto>>> GetDiagnosesByPatientCaseIdAsync(Guid patientCaseId, int page = 1, int pageSize = 10);
        Task<Result<DiagnosisDto>> CreateDiagnosisAsync(CreateDiagnosisDto dto, Guid? userId, string? userRole);


		Task<Result<DiagnosisDto>> UpdateDiagnosisAsync(UpdateDiagnosisDto dto);
        Task<Result> DeleteDiagnosisAsync(Guid id);
        Task<Result> AcceptDiagnosisAsync(Guid id);
    }
}
