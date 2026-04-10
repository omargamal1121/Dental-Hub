using DentalHub.Application.Commands.Diagnoses;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Diagnoses;
using DentalHub.Application.Services.DiagnosesServices;
using MediatR;

namespace DentalHub.Application.Handlers.Diagnoses
{
    public class CreateDiagnosisCommandHandler : IRequestHandler<CreateDiagnosisCommand, Result<DiagnosisDto>>
    {
        private readonly IDiagnosisService _service;

        public CreateDiagnosisCommandHandler(IDiagnosisService service)
        {
            _service = service;
        }

        public async Task<Result<DiagnosisDto>> Handle(CreateDiagnosisCommand request, CancellationToken cancellationToken)
        {
            return await _service.CreateDiagnosisAsync(request.ToDto(),request.CreatedById,request.Role);
        }
    }
}
