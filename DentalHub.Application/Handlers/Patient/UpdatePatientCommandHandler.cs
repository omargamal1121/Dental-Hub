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

        public Task<Result> Handle(UpdatePatientCommand request, CancellationToken ct)
            => _service.UpdateAsync(request);
    }
}
