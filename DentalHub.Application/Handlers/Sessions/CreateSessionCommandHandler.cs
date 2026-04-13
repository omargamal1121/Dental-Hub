using DentalHub.Application.Commands.Sessions;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, Result<bool>>
    {
        private readonly ISessionService _service;

        public CreateSessionCommandHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(CreateSessionCommand request, CancellationToken ct)
        {
            var dto = new CreateSessionDto
            {
                StudentId = request.StudentId,
                CaseId = request.PatientCaseId,
                ScheduledAt = request.SessionDate,
            };

            var result = await _service.CreateSessionAsync(dto);

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Errors ?? new List<string> { result.Message ?? "Session creation failed" }, result.Status);
            }

            return Result<bool>.Success(result.Data, result.Message, result.Status);
        }
    }
}
