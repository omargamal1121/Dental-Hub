using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Doctor
{
    public record CreateDoctorCommand(
        string Name,
        Guid UserId,
        string Email,
        string Password,
        string Specialty,
        int UniversityId
    ) : IRequest<Result<Guid>>;
}
