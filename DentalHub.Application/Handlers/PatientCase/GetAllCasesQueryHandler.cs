using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Queries.PatientCase;
using DentalHub.Application.Services.Cases;
using MediatR;

namespace DentalHub.Application.Handlers.PatientCase
{
    public class GetAllCasesQueryHandler
        : IRequestHandler<GetAllCasesQuery, Result<PagedResult<PatientCaseDto>>>
    {
        private readonly IPatientCaseService _service;

        public GetAllCasesQueryHandler(IPatientCaseService service)
        {
            _service = service;
        }

        public async Task<Result<PagedResult<PatientCaseDto>>> Handle(
            GetAllCasesQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetAllCasesAsync(
                request.Search,
                request.Status,
                request.Page,
                request.PageSize);
        }
    }
}
