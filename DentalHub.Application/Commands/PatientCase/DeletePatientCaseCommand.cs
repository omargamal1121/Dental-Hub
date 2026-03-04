using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.PatientCase
{
    public record DeletePatientCaseCommand(Guid Id) : IRequest<Result<bool>>;
}
