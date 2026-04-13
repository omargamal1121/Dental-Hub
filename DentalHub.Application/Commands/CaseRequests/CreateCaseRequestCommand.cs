using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Commands.CaseRequests
{
    public record CreateCaseRequestCommand(
        Guid PatientCasePublicId,
        Guid StudentPublicId,
        string DoctorUsername,
        string Description
    ) : IRequest<Result<Guid>>;
}