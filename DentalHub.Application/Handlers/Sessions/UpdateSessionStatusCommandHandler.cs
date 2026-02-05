using DentalHub.Application.Commands.Sessions;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class UpdateSessionStatusCommandHandler : IRequestHandler<UpdateSessionStatusCommand, Result<bool>>
    {
        private readonly ISessionService _service;

        public UpdateSessionStatusCommandHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(UpdateSessionStatusCommand request, CancellationToken ct)
        {
            var result = await _service.UpdateSessionStatusAsync(request.SessionId, request.Status);

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Errors ?? new List<string> { result.Message ?? "Update failed" }, result.Status);
            }

            return Result<bool>.Success(true, result.Message ?? "Session status updated successfully", result.Status);
        }
    }
}
