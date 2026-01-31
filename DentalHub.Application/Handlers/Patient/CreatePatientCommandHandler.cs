using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Common;

using DentalHub.Application.Services.Patient;
using MediatR;

namespace DentalHub.Application.Handlers.Patient
{
    public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Result<Guid>>
    {
        private readonly IPatientService _service;

        public CreatePatientCommandHandler(IPatientService service)
        {
            _service = service;
        }

        public Task<Result<Guid>> Handle(CreatePatientCommand request, CancellationToken ct)
            => _service.CreateAsync(request);
    }
}
