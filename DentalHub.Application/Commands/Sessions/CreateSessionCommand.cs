using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Sessions
{
    public record CreateSessionCommand(
        string StudentId,
        string PatientCaseId,
        DateTime SessionDate,
        string Location
    ) : IRequest<Result<string>>;
}
