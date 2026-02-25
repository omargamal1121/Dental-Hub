using DentalHub.Application.Common;
using DentalHub.Application.Queries.Auth;
using DentalHub.Application.Services.Admins;
using DentalHub.Application.Services.Doctors;
using DentalHub.Application.Services.Students;
using DentalHub.Application.Services;
using MediatR;

namespace DentalHub.Application.Handlers.Auth
{
   
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, Result<object>>
    {
        private readonly IDoctorService _doctorService;
        private readonly IStudentService _studentService;
        private readonly IPatientService _patientService;
        private readonly IAdminService _adminService;

        public GetMyProfileQueryHandler(
            IDoctorService doctorService,
            IStudentService studentService,
            IPatientService patientService,
            IAdminService adminService)
        {
            _doctorService = doctorService;
            _studentService = studentService;
            _patientService = patientService;
            _adminService = adminService;
        }

        public async Task<Result<object>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            return request.Role switch
            {
                "Doctor" => await GetDoctorProfile(request.UserId),
                "Student" => await GetStudentProfile(request.UserId),
                "Patient" => await GetPatientProfile(request.UserId),
                "Admin" or "SuperAdmin" => await GetAdminProfile(request.UserId),
                _ => Result<object>.Failure("Unknown role", 400)
            };
        }

        private async Task<Result<object>> GetDoctorProfile(string userId)
        {
           
            var result = await _doctorService.GetDoctorByIdAsync(userId);
            if (!result.IsSuccess)
                return Result<object>.Failure(result.Errors ?? new List<string> { result.Message ?? "Doctor not found" }, result.Status);

            return Result<object>.Success(result.Data, result.Message, result.Status);
        }

        private async Task<Result<object>> GetStudentProfile(string userId)
        {
            var result = await _studentService.GetStudentByUserIdAsync(userId);
            if (!result.IsSuccess)
                return Result<object>.Failure(result.Errors ?? new List<string> { result.Message ?? "Student not found" }, result.Status);

            return Result<object>.Success(result.Data, result.Message, result.Status);
        }

        private async Task<Result<object>> GetPatientProfile(string userId)
        {
            var result = await _patientService.GetPatientByUserIdAsync(userId);
            if (!result.IsSuccess)
                return Result<object>.Failure(result.Errors ?? new List<string> { result.Message ?? "Patient not found" }, result.Status);

            return Result<object>.Success(result.Data, result.Message, result.Status);
        }

        private async Task<Result<object>> GetAdminProfile(string userId)
        {
            var result = await _adminService.GetAdminByUserIdAsync(userId);
            if (!result.IsSuccess)
                return Result<object>.Failure(result.Errors ?? new List<string> { result.Message ?? "Admin not found" }, result.Status);

            return Result<object>.Success(result.Data, result.Message, result.Status);
        }
    }
}
