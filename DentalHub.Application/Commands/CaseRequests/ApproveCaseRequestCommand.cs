using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.CaseRequests
{
    public record ApproveCaseRequestCommand(
		Guid RequestPublicId,
		Guid DoctorPublicId
    ) : IRequest<Result<bool>>;
}
