using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Services.Cases
{
    public interface IPatientCaseService
    {
        // CRUD Operations
        Task<Result<PatientCaseDto>> CreateCaseAsync(CreateCaseDto dto);
        Task<Result<PatientCaseDto>> GetCaseByPublicIdAsync(string publicId);
        Task<Result<PagedResult<PatientCaseDto>>> GetAllCasesAsync(int page = 1, int pageSize = 10);
        Task<Result<PatientCaseDto>> UpdateCaseAsync(UpdateCaseDto dto);
        Task<Result> DeleteCaseByPublicIdAsync(string publicId);

        // Get cases by status
        Task<Result<PagedResult<PatientCaseDto>>> GetCasesByStatusAsync(
            string status, int page = 1, int pageSize = 10);

        // Get patient's cases
        Task<Result<PagedResult<PatientCaseDto>>> GetPatientCasesAsync(
            string patientPublicId, int page = 1, int pageSize = 10);

        // Change case status
        Task<Result<PatientCaseDto>> UpdateCaseStatusAsync(string publicId, string newStatus);

        /// <summary>
        /// ADDED: Combined method to update both treatment type and status in one transaction
        /// This prevents double database calls and ensures consistency
        /// </summary>
        Task<Result<PatientCaseDto>> UpdateCaseWithStatusAsync(
            string publicId,
            string? treatmentType,
            CaseStatus newStatus
        );
    }
}
