using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Queries.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class GetUpcomingSessionsQueryHandler
        : IRequestHandler<GetUpcomingSessionsQuery, Result<PagedResult<SessionDto>>>
    {
        private readonly ISessionService _service;

        public GetUpcomingSessionsQueryHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<PagedResult<SessionDto>>> Handle(
            GetUpcomingSessionsQuery request, CancellationToken ct)
        {
            return await _service.GetUpcomingSessionsAsync(
                request.Page, request.PageSize, request.StudentId, request.PatientId);
        }
    }
}
