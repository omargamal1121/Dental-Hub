using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Common;
using DentalHub.Application.Services.PatientCase;
using MediatR;

namespace DentalHub.Application.Handlers.PatientCase
{
    public class CreatePatientCaseCommandHandler : IRequestHandler<CreatePatientCaseCommand, Result<Guid>>
    {
        private readonly IPatientCaseService _service;

        public CreatePatientCaseCommandHandler(IPatientCaseService service)
        {
            _service = service;
        }

        public Task<Result<Guid>> Handle(CreatePatientCaseCommand request, CancellationToken ct)
            => _service.CreateAsync(request);
    }
}
