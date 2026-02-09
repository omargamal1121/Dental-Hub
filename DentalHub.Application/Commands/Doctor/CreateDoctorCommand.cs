using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Doctor
{
    public record CreateDoctorCommand(
        string Name,
        string Email,
        string Password,
        string Specialty,
        string UniversityId
    ) : IRequest<Result<Guid>>;
}
