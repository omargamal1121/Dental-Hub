using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;

namespace DentalHub.Application.Queries.Sessions
{
    public record GetSessionNotesQuery(string GuidSessionId) : IRequest<Result<List<SessionNoteDto>>>;
}
