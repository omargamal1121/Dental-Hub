using DentalHub.Application.Commands.Sessions;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class AddSessionNoteCommandHandler : IRequestHandler<AddSessionNoteCommand, Result<SessionNoteDto>>
    {
        private readonly ISessionService _service;

        public AddSessionNoteCommandHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<SessionNoteDto>> Handle(AddSessionNoteCommand request, CancellationToken ct)
        {
            var dto = new CreateSessionNoteDto
            {
                SessionId = request.SessionId,
                Note = request.Note
            };

            var result = await _service.AddSessionNoteAsync(dto);

            if (!result.IsSuccess)
                return Result<SessionNoteDto>.Failure(result.Errors ?? new List<string> { result.Message ?? "Add note failed" }, result.Status);

            return Result<SessionNoteDto>.Success(result.Data, result.Message, result.Status);
        }
    }
}
