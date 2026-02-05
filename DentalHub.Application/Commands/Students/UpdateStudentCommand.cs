using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Students
{
    public record UpdateStudentCommand(
        Guid UserId,
        string FullName,
        string University,
        int Grade
    ) : IRequest<Result<bool>>;
}
