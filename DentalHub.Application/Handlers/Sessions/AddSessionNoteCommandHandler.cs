using DentalHub.Application.Commands.Sessions;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class AddSessionNoteCommandHandler : IRequestHandler<AddSessionNoteCommand, Result<Guid>>
    {
        private readonly ISessionService _service;

        public AddSessionNoteCommandHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<Guid>> Handle(AddSessionNoteCommand request, CancellationToken ct)
        {
            var note= new CreateSessionNoteDto
            {
                SessionId = request.SessionId,
                Note = request.Note,

            };
			var result = await _service.AddSessionNoteAsync(note);
             if (!result.IsSuccess)
            {
                return Result<Guid>.Failure(result.Errors ?? new List<string> { result.Message ?? "Add note failed" }, result.Status);
            }
            return Result<Guid>.Success(result.Data.Id, result.Message, result.Status);
        }
    }
}
