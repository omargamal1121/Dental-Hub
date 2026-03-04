using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.CaseRequests
{
    public record CancelCaseRequestCommand(
		Guid RequestPublicId,
		Guid StudentPublicId
    ) : IRequest<Result>;
}
