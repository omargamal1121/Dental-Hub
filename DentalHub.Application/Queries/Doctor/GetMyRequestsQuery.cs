using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using MediatR;

namespace DentalHub.Application.Queries.Doctor
{
    /// Query to get case requests for the current logged-in doctor (from JWT token).
    public record GetMyRequestsQuery(string DoctorUserId, int Page, int PageSize)
        : IRequest<Result<PagedResult<CaseRequestDto>>>;
}
