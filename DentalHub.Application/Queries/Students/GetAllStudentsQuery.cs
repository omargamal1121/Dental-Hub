using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Students;

namespace DentalHub.Application.Queries.Students
{
    public record GetAllStudentsQuery(int Page = 1, int PageSize = 10) : IRequest<Result<List<StudentDto>>>;
}
