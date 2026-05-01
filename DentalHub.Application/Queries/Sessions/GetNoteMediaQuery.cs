using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;

namespace DentalHub.Application.Queries.Sessions
{
    public record GetNoteMediaQuery(Guid SessionId, Guid NoteId) : IRequest<Result<List<SessionMediaDto>>>;
}
