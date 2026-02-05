using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Patient
{
    public record CreatePatientCommand(
        string FullName,
        string Email,
        string Password,
        string PhoneNumber,
        string NationalId,
        DateTime BirthDate,
        Gender Gender
    ) : IRequest<Result<Guid>>;
}
