using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Sessions
{
    public record UpdateSessionStatusCommand(
        string SessionId,
        string Status
    ) : IRequest<Result<bool>>;
}
