using DentalHub.Application.Common;
using MediatR;

namespace DentalHub.Application.Commands.PatientCase
{
 public record CreatePatientCaseCommand(
    Guid PatientId,
    string TreatmentType
) : IRequest<Result<Guid>>;

}

