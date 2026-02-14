using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Queries.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class GetSessionsByStudentIdQueryHandler : IRequestHandler<GetSessionsByStudentIdQuery, Result<PagedResult<SessionDto>>>
    {
        private readonly ISessionService _service;

        public GetSessionsByStudentIdQueryHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<PagedResult<SessionDto>>> Handle(GetSessionsByStudentIdQuery request, CancellationToken ct)
        {
            return await _service.GetSessionsByStudentIdAsync(request.StudentId, request.Page, request.PageSize);
        }
    }
}
