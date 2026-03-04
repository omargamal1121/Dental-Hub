using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Patient
{
    public record UpdatePatientCommand(
		Guid PublicId,
        string FullName,
        string PhoneNumber,
        string NationalId,
        DateTime BirthDate,
        Gender Gender
    ) : IRequest<Result<bool>>;
}
