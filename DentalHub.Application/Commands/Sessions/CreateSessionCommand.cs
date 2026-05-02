using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Sessions
{
    public record CreateSessionCommand(
    Guid StudentId,
    Guid PatientCaseId,
    DateTime SessionDate,
    string Location,
    string? DoctorUsername = null
) : IRequest<Result<bool>>;
}
