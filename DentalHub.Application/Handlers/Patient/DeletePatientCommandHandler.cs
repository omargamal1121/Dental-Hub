using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Services.Patient;
using MediatR;

using DentalHub.Application.Common;
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
