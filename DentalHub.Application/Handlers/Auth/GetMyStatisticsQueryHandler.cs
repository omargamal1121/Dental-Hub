using DentalHub.Application.Common;
using DentalHub.Application.Queries.Auth;
using DentalHub.Application.Services.Doctors;
using DentalHub.Application.Services.Students;
using MediatR;

namespace DentalHub.Application.Handlers.Auth
{
    public class GetMyStatisticsQueryHandler : IRequestHandler<GetMyStatisticsQuery, Result<object>>
    {
        private readonly IDoctorService _doctorService;
        private readonly IStudentService _studentService;

        public GetMyStatisticsQueryHandler(
            IDoctorService doctorService,
            IStudentService studentService)
        {
            _doctorService = doctorService;
            _studentService = studentService;
        }

        public async Task<Result<object>> Handle(GetMyStatisticsQuery request, CancellationToken cancellationToken)
        {
            return request.Role switch
            {
                "Doctor" => await GetDoctorStats(request.UserId),
                "Student" => await GetStudentStats(request.UserId),
                _ => Result<object>.Failure("Statistics not available for this role", 400)
            };
        }

        private async Task<Result<object>> GetDoctorStats(string userId)
        {
            var result = await _doctorService.GetDoctorStatisticsAsync(userId);
            if (!result.IsSuccess)
                return Result<object>.Failure(result.Errors ?? new List<string> { result.Message ?? "Failed" }, result.Status);

            return Result<object>.Success(result.Data, result.Message, result.Status);
        }

        private async Task<Result<object>> GetStudentStats(string userId)
        {
            var result = await _studentService.GetStudentStatisticsAsync(userId);
            if (!result.IsSuccess)
                return Result<object>.Failure(result.Errors ?? new List<string> { result.Message ?? "Failed" }, result.Status);

            return Result<object>.Success(result.Data, result.Message, result.Status);
        }
    }
}
