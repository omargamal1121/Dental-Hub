using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Queries.CaseRequests
{
    public record GetCaseRequestByIdQuery(
        Guid PublicId,
        Guid CurrentUserId,
        bool IsAdmin = false
    ) : IRequest<Result<CaseRequestDto>>;
}
