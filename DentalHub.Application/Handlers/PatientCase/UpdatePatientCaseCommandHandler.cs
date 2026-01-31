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

        public Task<Result<bool>> Handle(UpdatePatientCaseCommand request, CancellationToken ct)
            => _service.UpdateAsync(request);
    }
}
