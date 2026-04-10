using DentalHub.Application.Commands.Diagnoses;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Diagnoses;
using DentalHub.Application.Services.DiagnosesServices;
using MediatR;

namespace DentalHub.Application.Handlers.Diagnoses
{
    public class UpdateDiagnosisCommandHandler : IRequestHandler<UpdateDiagnosisCommand, Result<DiagnosisDto>>
    {
        private readonly IDiagnosisService _service;

        public UpdateDiagnosisCommandHandler(IDiagnosisService service)
        {
            _service = service;
        }

        public async Task<Result<DiagnosisDto>> Handle(UpdateDiagnosisCommand request, CancellationToken cancellationToken)
        {
            return await _service.UpdateDiagnosisAsync(request.ToDto());
        }
    }
}
