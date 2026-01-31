using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Doctor;
using MediatR;

namespace DentalHub.Application.Handlers.Doctor
{
    public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, Result<Guid>>
    {
        private readonly IDoctorService _service;

        public UpdateDoctorCommandHandler(IDoctorService service)
        {
            _service = service;
        }

        public Task<Result<Guid>> Handle(UpdateDoctorCommand request, CancellationToken ct)
            => _service.UpdateAsync(request);
    }
}
