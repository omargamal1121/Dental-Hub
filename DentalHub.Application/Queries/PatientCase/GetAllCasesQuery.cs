using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using MediatR;

namespace DentalHub.Application.Queries.PatientCase
{
    /// <summary>
    /// Query to get all patient cases with optional search (patient name / case-type name)
    /// and optional status filter, plus pagination.
    /// </summary>
    public record GetAllCasesQuery(
        string PatientName = null,   // partial match on patient name
		string? Search   = null,   // partial match on patient name or case-type name
        string? Status   = null,   // filter by CaseStatus enum value
        int     Page     = 1,
        int     PageSize = 10
    ) : IRequest<Result<PagedResult<PatientCaseDto>>>;
}
