using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.CaseRequests
{
    public record ApproveCaseRequestCommand(
        string RequestPublicId,
        string DoctorPublicId
    ) : IRequest<Result<bool>>;
}
