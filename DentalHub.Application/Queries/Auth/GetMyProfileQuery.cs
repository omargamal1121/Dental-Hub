using DentalHub.Application.Common;
using MediatR;

namespace DentalHub.Application.Queries.Auth
{
    
    public record GetMyProfileQuery(Guid UserId, string Role) : IRequest<Result<object>>;
}
