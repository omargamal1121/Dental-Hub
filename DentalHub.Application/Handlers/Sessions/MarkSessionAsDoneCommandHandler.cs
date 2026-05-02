using DentalHub.Application.Commands.Sessions;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Sessions;
using MediatR;

namespace DentalHub.Application.Handlers.Sessions
{
    public class MarkSessionAsDoneCommandHandler : IRequestHandler<MarkSessionAsDoneCommand, Result<bool>>
    {
        private readonly ISessionService _sessionService;

        public MarkSessionAsDoneCommandHandler(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<Result<bool>> Handle(MarkSessionAsDoneCommand request, CancellationToken cancellationToken)
        {
            return await _sessionService.MarkSessionAsDoneAsync(request.SessionId, request.StudentId);
        }
    }
}
