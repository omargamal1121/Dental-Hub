using DentalHub.Application.Queries.Diagnoses;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Diagnoses;
using DentalHub.Application.Services.DiagnosesServices;
using MediatR;

namespace DentalHub.Application.Handlers.Diagnoses
{
    public class GetDiagnosesByPatientCaseIdQueryHandler : IRequestHandler<GetDiagnosesByPatientCaseIdQuery, Result<PagedResult<DiagnosisDto>>>
    {
        private readonly IDiagnosisService _service;

        public GetDiagnosesByPatientCaseIdQueryHandler(IDiagnosisService service)
        {
            _service = service;
        }

        public async Task<Result<PagedResult<DiagnosisDto>>> Handle(GetDiagnosesByPatientCaseIdQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetDiagnosesByPatientCaseIdAsync(request.PatientCaseId, request.Page, request.PageSize);
        }
    }
}
