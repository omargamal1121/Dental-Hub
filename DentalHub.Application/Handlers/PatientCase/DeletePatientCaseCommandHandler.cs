using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Cases;
using MediatR;

namespace DentalHub.Application.Handlers.PatientCase
{
    public class DeletePatientCaseCommandHandler : IRequestHandler<DeletePatientCaseCommand, Result<bool>>
    {
        private readonly IPatientCaseService _service;

        public DeletePatientCaseCommandHandler(IPatientCaseService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeletePatientCaseCommand request, CancellationToken ct)
        {
            var result = await _service.DeleteCaseAsync(request.Id);
            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Errors ?? new List<string> { result.Message ?? "Delete failed" }, result.Status);
            }
            return Result<bool>.Success(true, result.Message ?? "Case deleted successfully", result.Status);
        }
    }
}
