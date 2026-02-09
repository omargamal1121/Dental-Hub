using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Students
{
    public record CreateStudentCommand(
        string FullName,
        string Email,
        string Password,
        string UniversityId
    ) : IRequest<Result<Guid>>;
}
