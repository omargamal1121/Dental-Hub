using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;

namespace DentalHub.Application.Queries.Sessions
{
    public record GetAllSessionsQuery(int Page = 1, int PageSize = 10) : IRequest<Result<PagedResult<SessionDto>>>;
}
