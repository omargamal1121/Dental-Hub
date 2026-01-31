using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Patient;
using MediatR;

namespace DentalHub.Application.Handlers.Patient
{
    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, Result<bool>>
    {
        private readonly IPatientService _service;

        public UpdatePatientCommandHandler(IPatientService service)
        {
            _service = service;
        }

<<<<<<< HEAD
        public Task<Result<bool>> Handle(UpdatePatientCommand request, CancellationToken ct)
=======
        public Task<Result> Handle(UpdatePatientCommand request, CancellationToken ct)
>>>>>>> 06a39604c75770df99dfba2cd9260a57c8d96007
            => _service.UpdateAsync(request);
    }
}
