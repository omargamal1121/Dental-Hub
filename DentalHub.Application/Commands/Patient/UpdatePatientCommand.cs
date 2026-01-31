using DentalHub.Application.Common;
using MediatR;

namespace DentalHub.Application.Commands.Patient
{
    public record UpdatePatientCommand(
    Guid UserId,
    int Age,
    string Phone
) : IRequest<Result<bool>>;
}

