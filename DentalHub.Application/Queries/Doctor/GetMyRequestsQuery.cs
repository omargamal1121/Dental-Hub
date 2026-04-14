using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Domain.Entities;
using MediatR;

namespace DentalHub.Application.Queries.Doctor
{
    /// Query to get case requests for the current logged-in doctor (from JWT token).
    public record GetMyRequestsQuery(Guid DoctorUserId, int Page, int PageSize, RequestStatus? Status = null, string? SortDirection = "desc")
        : IRequest<Result<PagedResult<CaseRequestDto>>>;
}
