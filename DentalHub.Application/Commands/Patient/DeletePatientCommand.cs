using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Patient
{
    public record DeletePatientCommand(Guid PublicId) : IRequest<Result<bool>>;
}
