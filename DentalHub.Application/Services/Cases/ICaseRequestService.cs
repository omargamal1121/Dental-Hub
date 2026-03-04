using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Services.Cases
{
    public interface ICaseRequestService
    {
        // Create Request
        Task<Result<Guid>> CreateRequestAsync(CreateCaseRequestDto dto);

        // Get Requests
        Task<Result<PagedResult<CaseRequestDto>>> GetRequestsByDoctorIdAsync(
            Guid doctorId, int page = 1, int pageSize = 10);

        Task<Result<PagedResult<CaseRequestDto>>> GetRequestsByStudentIdAsync(
            Guid studentId, RequestStatus? status = null, int page = 1, int pageSize = 10);

        Task<Result<CaseRequestDto>> GetRequestByIdAsync(Guid id, Guid currentUserId, bool isadmin = false);

        // Approve/Reject
        Task<Result<bool>> ApproveRequstAsync(ApproveCaseRequestDto dto);

        Task<Result<bool>> RejectRequestAsync(Guid id, Guid doctorId);

        // Cancel Request
        Task<Result> CancelRequestAsync(Guid id, Guid studentId);

        // Bulk Operations
        Task<Result<bool>> RejectAllRequestsForCaseAsync(Guid caseId);

        Task<Result<bool>> MarkAllRequestsTakenForCaseAsync(
            Guid caseId, Guid approvedRequestId);

        Task<Result<IEnumerable<CaseRequestDto>>> GetRequestsByCaseIdAsync(
            Guid caseId, RequestStatus? status = null);

        Task<Result<bool>> CancelAllStudentRequestsAsync(Guid studentId);
    }
}
