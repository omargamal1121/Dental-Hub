using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Queries.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class GetSessionsByCaseIdQueryHandler : IRequestHandler<GetSessionsByCaseIdQuery, Result<PagedResult<SessionDto>>>
    {
        private readonly ISessionService _service;

        public GetSessionsByCaseIdQueryHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<PagedResult<SessionDto>>> Handle(GetSessionsByCaseIdQuery request, CancellationToken ct)
        {
            return await _service.GetSessionsByCaseIdAsync(request.CaseId, request.Page, request.PageSize);
        }
    }
}
