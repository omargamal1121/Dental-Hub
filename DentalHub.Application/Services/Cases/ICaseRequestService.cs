using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Services.Cases
{
    public interface ICaseRequestService
    {
        // Create Request
        Task<Result<CaseRequestDto>> CreateRequestAsync(CreateCaseRequestDto dto);

        // Get Requests
        Task<Result<PagedResult<CaseRequestDto>>> GetRequestsByDoctorIdAsync(Guid doctorId, int page = 1, int pageSize = 10);
        Task<Result<PagedResult<CaseRequestDto>>> GetRequestsByStudentIdAsync(Guid studentId, int page = 1, int pageSize = 10);
        Task<Result<CaseRequestDto>> GetRequestByIdAsync(Guid requestId);

        // Approve/Reject
        Task<Result<CaseRequestDto>> ApproveOrRejectRequestAsync(ApproveCaseRequestDto dto);

        // Cancel Request
        Task<Result> CancelRequestAsync(Guid requestId, Guid studentId);
    }
}
