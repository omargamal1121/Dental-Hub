using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Students;

namespace DentalHub.Application.Queries.Students
{
    public record GetStudentByIdQuery(Guid PublicId) : IRequest<Result<StudentDetailsDto>>;
}
