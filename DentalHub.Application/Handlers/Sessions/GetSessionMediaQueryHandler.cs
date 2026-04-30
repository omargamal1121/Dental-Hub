using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Queries.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class GetSessionMediaQueryHandler : IRequestHandler<GetSessionMediaQuery, Result<List<SessionMediaDto>>>
    {
        private readonly ISessionService _service;

        public GetSessionMediaQueryHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<List<SessionMediaDto>>> Handle(GetSessionMediaQuery request, CancellationToken ct)
        {
            return await _service.GetSessionMediaAsync(request.SessionId);
        }
    }
}
