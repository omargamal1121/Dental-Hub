using DentalHub.Application.Commands.Sessions;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class AddNoteMediaCommandHandler : IRequestHandler<AddNoteMediaCommand, Result<SessionMediaDto>>
    {
        private readonly ISessionService _service;

        public AddNoteMediaCommandHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<SessionMediaDto>> Handle(AddNoteMediaCommand request, CancellationToken ct)
        {
            var result = await _service.AddNoteMediaAsync(request.SessionId, request.NoteId, request.File);

            if (!result.IsSuccess)
                return Result<SessionMediaDto>.Failure(result.Errors ?? new List<string> { result.Message ?? "Upload failed" }, result.Status);

            return Result<SessionMediaDto>.Success(result.Data, result.Message, result.Status);
        }
    }
}
