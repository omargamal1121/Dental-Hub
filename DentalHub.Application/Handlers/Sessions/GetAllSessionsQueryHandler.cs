using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Queries.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class GetAllSessionsQueryHandler : IRequestHandler<GetAllSessionsQuery, Result<List<SessionDto>>>
    {
        private readonly ISessionService _service;

        public GetAllSessionsQueryHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<List<SessionDto>>> Handle(GetAllSessionsQuery request, CancellationToken ct)
        {
            return await _service.GetAllSessionsAsync(request.Page, request.PageSize);
        }
    }
}
