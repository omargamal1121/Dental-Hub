using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.PatientCase
{
    public record CreatePatientCaseCommand(
        Guid PatientId,
        string Title,
        string Description,
        int CaseTypeId
    ) : IRequest<Result<Guid>>;
}
