using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;

namespace DentalHub.Application.Queries.Sessions
{
    /// <summary>
    /// Returns sessions whose StartAt is in the future and status is Scheduled.
    /// Optional filter by studentId or patientId.
    /// </summary>
    public record GetUpcomingSessionsQuery(
        int Page = 1,
        int PageSize = 10,
        Guid? StudentId = null,
        Guid? PatientId = null
    ) : IRequest<Result<PagedResult<SessionDto>>>;
}
