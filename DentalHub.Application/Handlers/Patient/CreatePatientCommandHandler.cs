using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Identity;
using DentalHub.Application.DTOs.Identity;
using MediatR;
using DentalHub.Application.Services;

namespace DentalHub.Application.Handlers.PatientHandlers
{
    public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Result<Guid>>
    {
        private readonly IPatientService _service;

        public CreatePatientCommandHandler(IPatientService service)
        {
            _service = service;
        }

        public async Task<Result<Guid>> Handle(CreatePatientCommand request, CancellationToken ct)
        {
            return await _service.CreatePatientAsync(request);
        }
    }
}
