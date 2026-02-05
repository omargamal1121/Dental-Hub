using DentalHub.Application.Commands.Sessions;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class DeleteSessionCommandHandler : IRequestHandler<DeleteSessionCommand, Result<bool>>
    {
        private readonly ISessionService _service;

        public DeleteSessionCommandHandler(ISessionService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteSessionCommand request, CancellationToken ct)
        {
            var result = await _service.DeleteSessionAsync(request.Id);
            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Errors ?? new List<string> { result.Message ?? "Delete failed" }, result.Status);
            }
            return Result<bool>.Success(true, result.Message ?? "Session deleted successfully", result.Status);
        }
    }
}
