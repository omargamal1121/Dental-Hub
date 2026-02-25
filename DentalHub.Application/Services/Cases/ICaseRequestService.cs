using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Services.Cases
{
    public interface ICaseRequestService
    {
        // Create Request
        Task<Result<CaseRequestDto>> CreateRequestAsync(CreateCaseRequestDto dto);

        // Get Requests
        Task<Result<PagedResult<CaseRequestDto>>> GetRequestsByDoctorIdAsync(
            string doctorPublicId, int page = 1, int pageSize = 10);

        Task<Result<PagedResult<CaseRequestDto>>> GetRequestsByStudentIdAsync(
            string studentPublicId, RequestStatus? status = null, int page = 1, int pageSize = 10);

        Task<Result<CaseRequestDto>> GetRequestByPublicIdAsync(string publicId);

        // Approve/Reject
        Task<Result<bool>> ApproveRequstAsync(ApproveCaseRequestDto dto);

        Task<Result<bool>> RejectRequestAsync(string publicId, string doctorPublicId);

        // Cancel Request
        Task<Result> CancelRequestAsync(string publicId, string studentPublicId);

        // Bulk Operations
        Task<Result<bool>> RejectAllRequestsForCaseAsync(string casePublicId);

        Task<Result<bool>> MarkAllRequestsTakenForCaseAsync(
            string casePublicId, string approvedRequestPublicId);

        Task<Result<IEnumerable<CaseRequestDto>>> GetRequestsByCaseIdAsync(
            string casePublicId, RequestStatus? status = null);

        Task<Result<bool>> CancelAllStudentRequestsAsync(string studentPublicId);
    }
}
