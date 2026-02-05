using DentalHub.Application.Common;
using DentalHub.Application.Services.Students;
using DentalHub.Application.Queries.Students;
using MediatR;

namespace DentalHub.Application.Handlers.Students
{
    public class GetStudentStatisticsQueryHandler : IRequestHandler<GetStudentStatisticsQuery, Result<StudentStatsDto>>
    {
        private readonly IStudentService _service;

        public GetStudentStatisticsQueryHandler(IStudentService service)
        {
            _service = service;
        }

        public async Task<Result<StudentStatsDto>> Handle(GetStudentStatisticsQuery request, CancellationToken ct)
        {
            return await _service.GetStudentStatisticsAsync(request.StudentId);
        }
    }
}
