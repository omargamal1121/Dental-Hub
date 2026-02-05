using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Queries.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class GetSessionNotesQueryHandler : IRequestHandler<GetSessionNotesQuery, Result<List<SessionNoteDto>>>
    {
        private readonly ISessionService _service;

        public GetSessionNotesQueryHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<List<SessionNoteDto>>> Handle(GetSessionNotesQuery request, CancellationToken ct)
        {
            return await _service.GetSessionNotesAsync(request.SessionId);
        }
    }
}
