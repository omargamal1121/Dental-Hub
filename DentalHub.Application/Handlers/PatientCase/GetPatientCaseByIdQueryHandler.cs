using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Queries.PatientCase;
using DentalHub.Application.Services.Cases;
using MediatR;

namespace DentalHub.Application.Handlers.PatientCase
{
    public class GetPatientCaseByIdQueryHandler : IRequestHandler<GetPatientCaseByIdQuery, Result<PatientCaseDto>>
    {
        private readonly IPatientCaseService _service;

        public GetPatientCaseByIdQueryHandler(IPatientCaseService service)
        {
            _service = service;
        }

        public async Task<Result<PatientCaseDto>> Handle(GetPatientCaseByIdQuery request, CancellationToken ct)
        {
            return await _service.GetCaseByIdAsync(request.Id);
        }
    }
}
