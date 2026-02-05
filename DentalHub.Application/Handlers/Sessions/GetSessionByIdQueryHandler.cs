using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Queries.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class GetSessionByIdQueryHandler : IRequestHandler<GetSessionByIdQuery, Result<SessionDto>>
    {
        private readonly ISessionService _service;

        public GetSessionByIdQueryHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<SessionDto>> Handle(GetSessionByIdQuery request, CancellationToken ct)
        {
            return await _service.GetSessionByIdAsync(request.Id);
        }
    }
}
