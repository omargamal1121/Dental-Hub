using DentalHub.Application.Common;
using MediatR;

namespace DentalHub.Application.Queries.Auth
{
    /// Query to get statistics for the current logged-in user.
    public record GetMyStatisticsQuery(Guid UserId, string Role) : IRequest<Result<object>>;
}
