using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Students
{
    public record UpdateStudentCommand(
        string PublicId,
        string FullName,
        string University,
        int Level
    ) : IRequest<Result<bool>>;
}
