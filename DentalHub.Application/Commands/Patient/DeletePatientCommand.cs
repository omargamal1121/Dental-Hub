using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Patient
{
    public record DeletePatientCommand(string PublicId) : IRequest<Result<bool>>;
}
