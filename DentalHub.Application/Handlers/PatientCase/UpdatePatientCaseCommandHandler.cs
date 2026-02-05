using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Services.Cases;
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

        public async Task<Result<bool>> Handle(UpdatePatientCaseCommand request, CancellationToken ct)
        {
            var dto = new UpdateCaseDto
            {
                Id = request.Id,
             
                Description = request.Description,
           
            };

            var result = await _service.UpdateCaseAsync(dto);

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Errors ?? new List<string> { result.Message ?? "Update failed" }, result.Status);
            }

            return Result<bool>.Success(true, result.Message ?? "Case updated successfully", result.Status);
        }
    }
}
