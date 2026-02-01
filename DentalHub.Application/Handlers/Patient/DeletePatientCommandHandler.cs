using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Common;
using DentalHub.Application.Services.PatientServcie;
using MediatR;

namespace DentalHub.Application.Handlers.Patient
{
    public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, Result<bool>>
    {
        private readonly IPatientService _service;

        public DeletePatientCommandHandler(IPatientService service)
        {
            _service = service;
        }


        public Task<Result<bool>> Handle(DeletePatientCommand request, CancellationToken ct)

            => _service.DeleteAsync(request.UserId);
    }
}
