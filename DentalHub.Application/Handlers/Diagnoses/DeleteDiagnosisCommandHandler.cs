using DentalHub.Application.Commands.Diagnoses;
using DentalHub.Application.Common;
using DentalHub.Application.Services.DiagnosesServices;
using MediatR;

namespace DentalHub.Application.Handlers.Diagnoses
{
    public class DeleteDiagnosisCommandHandler : IRequestHandler<DeleteDiagnosisCommand, Result>
    {
        private readonly IDiagnosisService _service;

        public DeleteDiagnosisCommandHandler(IDiagnosisService service)
        {
            _service = service;
        }

        public async Task<Result> Handle(DeleteDiagnosisCommand request, CancellationToken cancellationToken)
        {
            return await _service.DeleteDiagnosisAsync(request.Id);
        }
    }
}
