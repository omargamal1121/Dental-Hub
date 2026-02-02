using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Cases;
using MediatR;

namespace DentalHub.Application.Handlers.PatientCase
{
    /// <summary>
    /// Now uses a single service call instead of two separate calls
    /// This prevents inconsistent state and reduces database roundtrips
    /// </summary>
    public class UpdatePatientCaseCommandHandler : IRequestHandler<UpdatePatientCaseCommand, Result<bool>>
    {
        private readonly IPatientCaseService _service;

        public UpdatePatientCaseCommandHandler(IPatientCaseService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(UpdatePatientCaseCommand request, CancellationToken ct)
        {
            // Use the new combined method that updates both treatment type and status in one transaction
            var result = await _service.UpdateCaseWithStatusAsync(
                request.Id,
                request.TreatmentType,
                request.Status
            );

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Message ?? "Update failed");
            }

            return Result<bool>.Success(true, "Case updated successfully");
        }
    }
}
