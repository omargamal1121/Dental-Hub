using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Queries.Students
{
    public record GetAvailableCasesForStudentQuery(
        string StudentPublicId,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<Result<PagedResult<PatientCaseDto>>>;
}
