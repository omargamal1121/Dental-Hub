using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Services.Patient;
using MediatR;

<<<<<<< HEAD
using DentalHub.Application.Common;
=======
>>>>>>> 06a39604c75770df99dfba2cd9260a57c8d96007
namespace DentalHub.Application.Handlers.Patient
{
    public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, Result<bool>>
    {
        private readonly IPatientService _service;

        public DeletePatientCommandHandler(IPatientService service)
        {
            _service = service;
        }

<<<<<<< HEAD
        public Task<Result<bool>> Handle(DeletePatientCommand request, CancellationToken ct)
=======
        public Task<Result> Handle(DeletePatientCommand request, CancellationToken ct)
>>>>>>> 06a39604c75770df99dfba2cd9260a57c8d96007
            => _service.DeleteAsync(request.UserId);
    }
}
