using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Queries.PatientCase;
using DentalHub.Application.Services.Cases;
using MediatR;

namespace DentalHub.Application.Handlers.PatientCase
{
    /// Handler to get all cases for a specific patient
    public class GetPatientCasesByPatientIdQueryHandler : IRequestHandler<GetPatientCasesByPatientIdQuery, Result<PagedResult<PatientCaseDto>>>
    {
        private readonly IPatientCaseService _service;

        public GetPatientCasesByPatientIdQueryHandler(IPatientCaseService service)
        {
            _service = service;
        }

        public async Task<Result<PagedResult<PatientCaseDto>>> Handle(GetPatientCasesByPatientIdQuery request, CancellationToken ct)
        {
            // FIX: Use GetPatientCasesAsync instead of GetCaseByIdAsync
            return await _service.GetPatientCasesAsync(request.PatientId, request.Page, request.PageSize);
        }
    }
}
