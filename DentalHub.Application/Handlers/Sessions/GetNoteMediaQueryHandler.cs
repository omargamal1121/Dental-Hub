using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Queries.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class GetNoteMediaQueryHandler : IRequestHandler<GetNoteMediaQuery, Result<List<SessionMediaDto>>>
    {
        private readonly ISessionService _service;

        public GetNoteMediaQueryHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<List<SessionMediaDto>>> Handle(GetNoteMediaQuery request, CancellationToken ct)
        {
            return await _service.GetNoteMediaAsync(request.NoteId);
        }
    }
}
