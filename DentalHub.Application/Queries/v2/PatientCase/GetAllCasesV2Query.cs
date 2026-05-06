using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.DTOs.v2.Cases;
using MediatR;

namespace DentalHub.Application.Queries.v2.PatientCase
{
    public record GetAllCasesV2Query(CaseFilterV2Dto Filter) : IRequest<Result<PagedResult<PatientCaseDto>>>;
}
