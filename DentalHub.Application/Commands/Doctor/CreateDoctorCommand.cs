using MediatR;
<<<<<<< HEAD
using DentalHub.Application.Common;
=======

using DentalHub.Application.Common.DentalHub.Domain.Common;
>>>>>>> 06a39604c75770df99dfba2cd9260a57c8d96007
namespace DentalHub.Application.Commands.Doctor
{
    public record CreateDoctorCommand(
    string Name,
    Guid UserId,
    string Specialty,
    int UniversityId
) : IRequest<Result<Guid>>;

}
