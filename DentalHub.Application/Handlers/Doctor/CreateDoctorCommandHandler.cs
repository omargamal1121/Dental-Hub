using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Common;
<<<<<<< HEAD
=======
using DentalHub.Application.Common.DentalHub.Domain.Common;
>>>>>>> 06a39604c75770df99dfba2cd9260a57c8d96007
using DentalHub.Application.Services.Doctor;
using MediatR;

namespace DentalHub.Application.Handlers.Doctor
{
    public class CreateDoctorCommandHandler : IRequestHandler<CreateDoctorCommand, Result<Guid>>
    {
        private readonly IDoctorService _service;

        public CreateDoctorCommandHandler(IDoctorService service)
        {
            _service = service;
        }

        public Task<Result<Guid>> Handle(CreateDoctorCommand request, CancellationToken ct)
            => _service.CreateAsync(request);
    }
}
