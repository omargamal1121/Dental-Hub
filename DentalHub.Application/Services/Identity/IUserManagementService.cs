using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Identity;
using DentalHub.Application.DTOs.Doctors;

namespace DentalHub.Application.Services.Identity
{
    
    public interface IUserManagementService
    {
        

        Task<Result<AuthResponseDto>> RegisterStudentAsync(RegisterStudentDto dto);
        Task<Result<AuthResponseDto>> RegisterDoctorAsync(RegisterDoctorDto dto);

        // Login
       // Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
        //Task<Result> LogoutAsync();

        // User Management
      //  Task<Result<bool>> CheckEmailExistsAsync(string email);
        Task<Result> DeleteUserAsync(Guid userId);
        Task<Result<List<DoctorlistDto>>> GetClinicalDoctorsAsync();
    }
}
