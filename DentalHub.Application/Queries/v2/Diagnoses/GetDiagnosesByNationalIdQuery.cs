using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Diagnoses;
using MediatR;

namespace DentalHub.Application.Queries.v2.Diagnoses
{
    public record GetDiagnosesByNationalIdQuery(string NationalId, int Page = 1, int PageSize = 10) 
        : IRequest<Result<PagedResult<DiagnosisDto>>>;
}
