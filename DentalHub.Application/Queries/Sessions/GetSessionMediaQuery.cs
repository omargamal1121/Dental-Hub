using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;

namespace DentalHub.Application.Queries.Sessions
{
    public record GetSessionMediaQuery(Guid SessionId) : IRequest<Result<List<SessionMediaDto>>>;
}
