using DentalHub.Application.Commands.Sessions;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Sessions;
using MediatR;

public class EvaluateSessionCommandHandler : IRequestHandler<EvaluateSessionCommand, Result<Guid>>
{
    private readonly ISessionService _sessionService;

    public EvaluateSessionCommandHandler(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public async Task<Result<Guid>> Handle(EvaluateSessionCommand request, CancellationToken cancellationToken)
    {
        if (request.Grade < 0 || request.Grade > 20)
        {
            return Result<Guid>.Failure("Invalid grade. The grade must be between 0 and 20.");
        }

        return await _sessionService.EvaluateSessionAsync(
            request.SessionId,
            request.DoctorId,
            request.Grade,
            request.Note,
            request.IsFinalSession);
    }
}
