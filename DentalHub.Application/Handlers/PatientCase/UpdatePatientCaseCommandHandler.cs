using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Common;
using DentalHub.Application.Services.PatientCase;
using MediatR;

namespace DentalHub.Application.Handlers.PatientCase
{
    public class UpdatePatientCaseCommandHandler : IRequestHandler<UpdatePatientCaseCommand, Result<bool>>
    {
        private readonly IPatientCaseService _service;

        public UpdatePatientCaseCommandHandler(IPatientCaseService service)
        {
            _service = service;
        }

<<<<<<< HEAD
        public Task<Result<bool>> Handle(UpdatePatientCaseCommand request, CancellationToken ct)
=======
        public Task<Result> Handle(UpdatePatientCaseCommand request, CancellationToken ct)
>>>>>>> 06a39604c75770df99dfba2cd9260a57c8d96007
            => _service.UpdateAsync(request);
    }
}
