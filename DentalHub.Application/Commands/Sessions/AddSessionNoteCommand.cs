using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Sessions
{
    public record AddSessionNoteCommand(
        Guid SessionId,
        string Note,
        bool IsPrivate
    ) : IRequest<Result<Guid>>;
}
