using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Queries.Students
{
    public record GetAvailableCasesForStudentQuery(
		Guid StudentPublicId,
        CaseFilterDto Filter
    ) : IRequest<Result<PagedResult<AvailableCasesDto>>>;
}
