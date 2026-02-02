using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Services.Cases
{
    public interface IPatientCaseService
    {
        // CRUD Operations
        Task<Result<PatientCaseDto>> CreateCaseAsync(CreateCaseDto dto);
        Task<Result<PatientCaseDto>> GetCaseByIdAsync(Guid caseId);
        Task<Result<List<PatientCaseDto>>> GetAllCasesAsync(int page = 1, int pageSize = 10);
        Task<Result<PatientCaseDto>> UpdateCaseAsync(UpdateCaseDto dto);
        Task<Result> DeleteCaseAsync(Guid caseId);

        // Get cases by status
        Task<Result<List<PatientCaseDto>>> GetCasesByStatusAsync(
            string status, int page = 1, int pageSize = 10);

        // Get patient's cases
        Task<Result<List<PatientCaseDto>>> GetPatientCasesAsync(
            Guid patientId, int page = 1, int pageSize = 10);

        // Change case status
        Task<Result<PatientCaseDto>> UpdateCaseStatusAsync(Guid caseId, string newStatus);

        /// <summary>
        /// ADDED: Combined method to update both treatment type and status in one transaction
        /// This prevents double database calls and ensures consistency
        /// </summary>
        Task<Result<PatientCaseDto>> UpdateCaseWithStatusAsync(
            Guid caseId,
            string? treatmentType,
            CaseStatus newStatus
        );
    }
}
