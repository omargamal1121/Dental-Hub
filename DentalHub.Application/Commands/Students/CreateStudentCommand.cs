using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Students
{
    public record CreateStudentCommand(
        string FullName,
        string Email,
        string Password,
        Guid UniversityId,
        int Level,
        string Username,
        string Phone
	) : IRequest<Result<Guid>>;
}
