using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using Microsoft.AspNetCore.Http;

namespace DentalHub.Application.Commands.Sessions
{
    // Upload media to a session directly
    public record AddSessionMediaCommand(
        Guid SessionId,
        IFormFile File
    ) : IRequest<Result<SessionMediaDto>>;

    // Upload media to a specific note inside a session
    public record AddNoteMediaCommand(
        Guid SessionId,
        Guid NoteId,
        IFormFile File
    ) : IRequest<Result<SessionMediaDto>>;
}
