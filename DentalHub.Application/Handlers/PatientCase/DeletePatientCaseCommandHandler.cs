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
                return Result<bool>.Failure(result.Message ?? "Delete failed");
            }

            return Result<bool>.Success(true, "Case deleted successfully");
        }
    }
}
