using DentalHub.Application.Commands.Sessions;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class AddSessionMediaCommandHandler : IRequestHandler<AddSessionMediaCommand, Result<SessionMediaDto>>
    {
        private readonly ISessionService _service;

        public AddSessionMediaCommandHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<SessionMediaDto>> Handle(AddSessionMediaCommand request, CancellationToken ct)
        {
            var result = await _service.AddSessionMediaAsync(request.SessionId, request.File);

            if (!result.IsSuccess)
                return Result<SessionMediaDto>.Failure(result.Errors ?? new List<string> { result.Message ?? "Upload failed" }, result.Status);

            return Result<SessionMediaDto>.Success(result.Data, result.Message, result.Status);
        }
    }
}
