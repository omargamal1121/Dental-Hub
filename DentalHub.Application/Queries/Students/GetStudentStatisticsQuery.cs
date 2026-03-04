using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Students;

namespace DentalHub.Application.Queries.Students
{
    public record GetStudentStatisticsQuery(Guid StudentPublicId) : IRequest<Result<StudentStatsDto>>;
}
