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
            var result = await _service.DeletePatientAsync(request.Id);
            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Errors ?? new List<string> { result.Message ?? "Delete failed" }, result.Status);
            }
            return Result<bool>.Success(true, result.Message ?? "Patient deleted successfully", result.Status);
        }
    }
}
