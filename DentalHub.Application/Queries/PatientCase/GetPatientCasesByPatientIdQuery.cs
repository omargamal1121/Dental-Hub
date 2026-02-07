using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using MediatR;

namespace DentalHub.Application.Queries.PatientCase
{
    /// Query to get all cases for a specific patient with pagination
    public record GetPatientCasesByPatientIdQuery(
        Guid PatientId,
        int Page = 1,
        int PageSize = 10
    ) : IRequest<Result<List<PatientCaseDto>>>;
}
