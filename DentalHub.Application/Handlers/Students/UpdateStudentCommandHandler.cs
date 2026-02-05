using DentalHub.Application.Commands.Students;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Students;
using DentalHub.Application.Services.Students;
using MediatR;

namespace DentalHub.Application.Handlers.Students
{
    public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Result<bool>>
    {
        private readonly IStudentService _service;

        public UpdateStudentCommandHandler(IStudentService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(UpdateStudentCommand request, CancellationToken ct)
        {
            var dto = new UpdateStudentDto
            {
                UserId = request.UserId,
                FullName = request.FullName,
                University = request.University,
               
            };

            var result = await _service.UpdateStudentAsync(dto);

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Errors ?? new List<string> { result.Message ?? "Update failed" }, result.Status);
            }

            return Result<bool>.Success(true, result.Message ?? "Student updated successfully", result.Status);
        }
    }
}
