using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Doctor
{
    public record UpdateDoctorCommand(
		Guid PublicId,
        string Name,
        string Specialty,
        int UniversityId
    ) : IRequest<Result<bool>>;
}
