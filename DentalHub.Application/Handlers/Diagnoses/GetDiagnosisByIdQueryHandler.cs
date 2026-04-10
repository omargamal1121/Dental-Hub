using DentalHub.Application.Queries.Diagnoses;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Diagnoses;
using DentalHub.Application.Services.DiagnosesServices;
using MediatR;

namespace DentalHub.Application.Handlers.Diagnoses
{
    public class GetDiagnosisByIdQueryHandler : IRequestHandler<GetDiagnosisByIdQuery, Result<DiagnosisDto>>
    {
        private readonly IDiagnosisService _service;

        public GetDiagnosisByIdQueryHandler(IDiagnosisService service)
        {
            _service = service;
        }

        public async Task<Result<DiagnosisDto>> Handle(GetDiagnosisByIdQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetDiagnosisByIdAsync(request.Id);
        }
    }
}
