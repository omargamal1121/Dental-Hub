using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.CaseRequests
{
    public record RejectAllRequestsForCaseCommand(
		Guid CasePublicId
    ) : IRequest<Result<bool>>;
}
