using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.CaseRequests
{
    public record MarkAllRequestsTakenForCaseCommand(
		Guid CasePublicId,
		Guid ApprovedRequestPublicId
    ) : IRequest<Result<bool>>;
}
