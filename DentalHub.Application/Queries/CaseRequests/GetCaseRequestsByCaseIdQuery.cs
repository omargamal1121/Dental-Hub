using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Domain.Entities;

namespace DentalHub.Application.Queries.CaseRequests
{
    public record GetCaseRequestsByCaseIdQuery(
        string CasePublicId,
        RequestStatus? Status = null
    ) : IRequest<Result<IEnumerable<CaseRequestDto>>>;
}
