using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Sessions
{
    public record EvaluateSessionCommand(
       Guid SessionId,
       Guid DoctorId,
       int Grade,
       string Note,
       bool IsFinalSession) : IRequest<Result<Guid>>;
}