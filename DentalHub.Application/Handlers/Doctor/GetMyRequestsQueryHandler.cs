using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Queries.Doctor;
using DentalHub.Application.Services.Cases;
using MediatR;

namespace DentalHub.Application.Handlers.Doctor
{
    public class GetMyRequestsQueryHandler : IRequestHandler<GetMyRequestsQuery, Result<PagedResult<CaseRequestDto>>>
    {
        private readonly ICaseRequestService _caseRequestService;

        public GetMyRequestsQueryHandler(ICaseRequestService caseRequestService)
        {
            _caseRequestService = caseRequestService;
        }

        public async Task<Result<PagedResult<CaseRequestDto>>> Handle(
            GetMyRequestsQuery request, CancellationToken cancellationToken)
        {
            return await _caseRequestService.GetRequestsByDoctorIdAsync(
                request.DoctorUserId,
                request.Page,
                request.PageSize);
        }
    }
}
