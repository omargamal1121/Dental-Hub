using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Identity;

namespace DentalHub.Application.Services.Identity
{
    /// <summary>
    /// Interface for authentication and user management services
    /// </summary>
    public interface IUserManagementService
    {
        // Registration
        Task<Result<AuthResponseDto>> RegisterPatientAsync(RegisterPatientDto dto);
        Task<Result<AuthResponseDto>> RegisterStudentAsync(RegisterStudentDto dto);
        Task<Result<AuthResponseDto>> RegisterDoctorAsync(RegisterDoctorDto dto);

        // Login
       // Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
        //Task<Result> LogoutAsync();

        // User Management
        Task<Result<bool>> CheckEmailExistsAsync(string email);
        Task<Result> DeleteUserAsync(Guid userId);
    }
}
