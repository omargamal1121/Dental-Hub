using DentalHub.Application.Common;
using MediatR;

namespace DentalHub.Application.Commands.Sessions
{
    public record MarkSessionAsDoneCommand(Guid SessionId, Guid StudentId) : IRequest<Result<bool>>;
}
