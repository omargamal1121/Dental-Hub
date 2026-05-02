using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Queries.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class GetSessionsNeedingEvaluationQueryHandler : IRequestHandler<GetSessionsNeedingEvaluationQuery, Result<PagedResult<SessionDto>>>
    {
        private readonly ISessionService _sessionService;

        public GetSessionsNeedingEvaluationQueryHandler(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<Result<PagedResult<SessionDto>>> Handle(GetSessionsNeedingEvaluationQuery request, CancellationToken cancellationToken)
        {
            return await _sessionService.GetSessionsNeedingEvaluationAsync(
                request.DoctorId, request.StudentId, request.PatientId, request.Page, request.PageSize);
        }
    }
}
