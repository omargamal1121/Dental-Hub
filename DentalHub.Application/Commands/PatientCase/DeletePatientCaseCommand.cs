using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.PatientCase
{
    public record DeletePatientCaseCommand(string Id) : IRequest<Result<bool>>;
}
