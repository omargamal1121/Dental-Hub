using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.PatientCase
{
    public record CreatePatientCaseCommand(
        string PatientId,
        string Title,
        string Description,
        string CaseTypeId
    ) : IRequest<Result<string>>;
}
