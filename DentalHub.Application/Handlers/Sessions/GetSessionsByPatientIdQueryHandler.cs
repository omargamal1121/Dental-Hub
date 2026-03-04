using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Queries.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class GetSessionsByPatientIdQueryHandler : IRequestHandler<GetSessionsByPatientIdQuery, Result<PagedResult<SessionDto>>>
    {
        private readonly ISessionService _service;

        public GetSessionsByPatientIdQueryHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<PagedResult<SessionDto>>> Handle(GetSessionsByPatientIdQuery request, CancellationToken ct)
        {
            return await _service.GetSessionsByPatientIdAsync(request.PatientId, request.Page, request.PageSize);
        }
    }
}
