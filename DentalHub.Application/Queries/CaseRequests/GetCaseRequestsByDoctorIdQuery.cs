using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Queries.CaseRequests
{
    public record GetCaseRequestsByDoctorIdQuery(
        string DoctorPublicId,
        int Page = 1,
        int PageSize = 10
    ) : IRequest<Result<PagedResult<CaseRequestDto>>>;
}
