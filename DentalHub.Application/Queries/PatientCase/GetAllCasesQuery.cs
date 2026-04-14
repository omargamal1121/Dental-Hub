using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using MediatR;

namespace DentalHub.Application.Queries.PatientCase
{
    /// <summary>
    /// Query to get all patient cases with optional search (patient name / case-type name)
    /// and optional status filter, plus pagination.
    /// </summary>
    public record GetAllCasesQuery(CaseFilterDto Filter) : IRequest<Result<PagedResult<PatientCaseDto>>>;
}
