using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Common;
using DentalHub.Application.Services.PatientCase;
using MediatR;

namespace DentalHub.Application.Handlers.PatientCase
{
    public class DeletePatientCaseCommandHandler : IRequestHandler<DeletePatientCaseCommand, Result<bool>>
    {
        private readonly IPatientCaseService _service;

        public DeletePatientCaseCommandHandler(IPatientCaseService service)
        {
            _service = service;
        }

        public Task<Result<bool>> Handle(DeletePatientCaseCommand request, CancellationToken ct)
            => _service.DeleteAsync(request.Id);
    }
}
