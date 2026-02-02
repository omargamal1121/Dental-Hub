using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Patients;
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

        public async Task<Result<bool>> Handle(DeletePatientCommand request, CancellationToken ct)
        {
            var result = await _service.DeletePatientAsync(request.UserId);

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Message ?? "Delete failed");
            }

            return Result<bool>.Success(true, "Patient deleted successfully");
        }
    }
}
