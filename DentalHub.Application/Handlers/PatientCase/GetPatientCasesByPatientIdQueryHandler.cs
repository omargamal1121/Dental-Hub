using DentalHub.Application.Common;
using DentalHub.Application.DTOs;
using DentalHub.Application.Queries.PatientCase;
using DentalHub.Application.Services.PatientCase;
using MediatR;

namespace DentalHub.Application.Handlers.PatientCase
{
    public class GetPatientCasesByPatientIdQueryHandler : IRequestHandler<GetPatientCasesByPatientIdQuery, Result<List<PatientCaseDto>>>
    {
        private readonly IPatientCaseService _service;

        public GetPatientCasesByPatientIdQueryHandler(IPatientCaseService service)
        {
            _service = service;
        }

        public Task<Result<List<PatientCaseDto>>> Handle(GetPatientCasesByPatientIdQuery request, CancellationToken ct)
            => _service.GetByPatientIdAsync(request.PatientId);
    }
}
