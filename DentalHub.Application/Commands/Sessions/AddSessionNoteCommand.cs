using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;

namespace DentalHub.Application.Commands.Sessions
{
    public record AddSessionNoteCommand(
        Guid SessionId,
        string Note
    ) : IRequest<Result<SessionNoteDto>>;
}
