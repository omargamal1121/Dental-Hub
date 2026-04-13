using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Queries.CaseRequests
{
    public record GetCaseRequestsByDoctorIdQuery(
		Guid DoctorPublicId,
        RequestStatus? Status,
        int Page = 1,
        int PageSize = 10,
        string Sort ="asc" 
        
    ) : IRequest<Result<PagedResult<CaseRequestDto>>>;
}
