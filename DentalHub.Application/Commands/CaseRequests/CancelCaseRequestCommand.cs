using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.CaseRequests
{
    public record CancelCaseRequestCommand(
        string RequestPublicId,
        string StudentPublicId
    ) : IRequest<Result>;
}
