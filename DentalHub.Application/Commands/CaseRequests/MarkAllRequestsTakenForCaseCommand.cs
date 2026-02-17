using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.CaseRequests
{
    public record MarkAllRequestsTakenForCaseCommand(
        string CasePublicId,
        string ApprovedRequestPublicId
    ) : IRequest<Result<bool>>;
}
