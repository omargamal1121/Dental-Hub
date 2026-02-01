using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Common;
<<<<<<< HEAD
using DentalHub.Application.Services.Doctor;
using MediatR;
using DentalHub.Application.Handlers;
namespace DentalHub.Application.Handlers.Doctor
{
    public class DeleteDoctorCommandHandler : IRequestHandler<DeleteDoctorCommand, Result<Guid>>
=======
using DentalHub.Application.Common.DentalHub.Domain.Common;
using DentalHub.Application.Services.Doctor;
using MediatR;

namespace DentalHub.Application.Handlers.Doctor
{
    public class DeleteDoctorCommandHandler : IRequestHandler<DeleteDoctorCommand, Result>
>>>>>>> 06a39604c75770df99dfba2cd9260a57c8d96007
    {
        private readonly IDoctorService _service;

        public DeleteDoctorCommandHandler(IDoctorService service)
        {
            _service = service;
        }

<<<<<<< HEAD
        public Task<Result<Guid>> Handle(DeleteDoctorCommand request, CancellationToken ct)
=======
        public Task<Result> Handle(DeleteDoctorCommand request, CancellationToken ct)
>>>>>>> 06a39604c75770df99dfba2cd9260a57c8d96007
            => _service.DeleteAsync(request.Id);
    }
}
