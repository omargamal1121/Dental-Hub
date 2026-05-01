using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Cases;
using MediatR;

namespace DentalHub.Application.Handlers.PatientCase
{
    public class UpdatePatientCaseStatusCommandHandler : IRequestHandler<UpdatePatientCaseStatusCommand, Result<bool>>
    {
        private readonly IPatientCaseService _service;

        public UpdatePatientCaseStatusCommandHandler(IPatientCaseService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(UpdatePatientCaseStatusCommand request, CancellationToken ct)
        {
            var result = await _service.UpdateCaseStatusAsync(request.Id, request.Status);

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Errors ?? new List<string> { result.Message ?? "Status update failed" }, result.Status);
            }

            return Result<bool>.Success(true, "Case status updated successfully", result.Status);
        }
    }
}
