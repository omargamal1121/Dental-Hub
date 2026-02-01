using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Common;

using DentalHub.Application.Services.DoctorService;
using MediatR;

namespace DentalHub.Application.Handlers.Doctor
{

    public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, Result<bool>>

    {
        private readonly IDoctorService _service;

        public UpdateDoctorCommandHandler(IDoctorService service)
        {
            _service = service;
        }


        public Task<Result<bool>> Handle(UpdateDoctorCommand request, CancellationToken ct)

            => _service.UpdateAsync(request);
    }
}
