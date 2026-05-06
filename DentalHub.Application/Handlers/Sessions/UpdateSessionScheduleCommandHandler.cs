using DentalHub.Application.Common;
using DentalHub.Application.Commands.Sessions;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class UpdateSessionScheduleCommandHandler : IRequestHandler<UpdateSessionScheduleCommand, Result<bool>>
    {
        private readonly ISessionService _sessionService;

        public UpdateSessionScheduleCommandHandler(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<Result<bool>> Handle(UpdateSessionScheduleCommand request, CancellationToken cancellationToken)
        {
            return await _sessionService.UpdateSessionScheduleAsync(request.SessionId, request.NewScheduledAt, request.StudentId);
        }
    }
}
