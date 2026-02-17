using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Commands.CaseRequests
{
    public record CreateCaseRequestCommand(
        string PatientCasePublicId,
        string StudentPublicId,
        string DoctorPublicId,
        string Description
    ) : IRequest<Result<CaseRequestDto>>;
}
