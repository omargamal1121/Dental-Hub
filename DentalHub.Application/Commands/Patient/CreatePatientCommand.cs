using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Patient
{
    public record CreatePatientCommand(
        string FullName,
    
        string Password,
        string PhoneNumber,
        string NationalId,
        DateTime BirthDate,
        Gender Gender,
        City City
    ) : IRequest<Result<Guid>>;
}
