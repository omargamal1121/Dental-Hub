using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.PatientCase
{
    public record UpdatePatientCaseCommand(
        Guid Id,
        string Title,
        string Description,
        int CaseTypeId,
        string Status
    ) : IRequest<Result<bool>>;
}
