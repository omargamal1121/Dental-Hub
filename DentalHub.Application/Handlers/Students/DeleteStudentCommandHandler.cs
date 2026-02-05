using DentalHub.Application.Commands.Students;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Students;
using MediatR;

namespace DentalHub.Application.Handlers.Students
{
    public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, Result<bool>>
    {
        private readonly IStudentService _service;

        public DeleteStudentCommandHandler(IStudentService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteStudentCommand request, CancellationToken ct)
        {
            var result = await _service.DeleteStudentAsync(request.Id);
            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Errors ?? new List<string> { result.Message ?? "Delete failed" }, result.Status);
            }
            return Result<bool>.Success(true, result.Message ?? "Student deleted successfully", result.Status);
        }
    }
}
