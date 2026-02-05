using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Queries.Students
{
    public record GetAvailableCasesForStudentQuery(Guid StudentId, int Page = 1, int PageSize = 10) : IRequest<Result<List<PatientCaseDto>>>;
}
