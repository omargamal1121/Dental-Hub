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
<<<<<<< HEAD
    public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, Result<Guid>>
=======
    public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, Result>
>>>>>>> 06a39604c75770df99dfba2cd9260a57c8d96007
    {
        private readonly IDoctorService _service;

        public UpdateDoctorCommandHandler(IDoctorService service)
        {
            _service = service;
        }

<<<<<<< HEAD
        public Task<Result<Guid>> Handle(UpdateDoctorCommand request, CancellationToken ct)
=======
        public Task<Result> Handle(UpdateDoctorCommand request, CancellationToken ct)
>>>>>>> 06a39604c75770df99dfba2cd9260a57c8d96007
            => _service.UpdateAsync(request);
    }
}
