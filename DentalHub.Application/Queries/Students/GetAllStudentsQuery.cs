using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Students;

namespace DentalHub.Application.Queries.Students
{
    public record GetAllStudentsQuery(int Page = 1, int PageSize = 10, string? Search = null, int? Level = null) : IRequest<Result<PagedResult<StudentDto>>>;
}
