using DentalHub.Application.Commands.Diagnoses;
using DentalHub.Application.Common;
using DentalHub.Application.Services.DiagnosesServices;
using MediatR;

namespace DentalHub.Application.Handlers.Diagnoses
{
    public class AcceptDiagnosisCommandHandler : IRequestHandler<AcceptDiagnosisCommand, Result>
    {
        private readonly IDiagnosisService _service;

        public AcceptDiagnosisCommandHandler(IDiagnosisService service)
        {
            _service = service;
        }

        public async Task<Result> Handle(AcceptDiagnosisCommand request, CancellationToken cancellationToken)
        {
            return await _service.AcceptDiagnosisAsync(request.Id);
        }
    }
}
