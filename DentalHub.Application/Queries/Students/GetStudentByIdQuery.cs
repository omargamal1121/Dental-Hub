using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Students;

namespace DentalHub.Application.Queries.Students
{
    public record GetStudentByIdQuery(string PublicId) : IRequest<Result<StudentDto>>;
}
