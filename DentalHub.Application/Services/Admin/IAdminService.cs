using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Admins;

namespace DentalHub.Application.Services.Admins
{
    /// Service interface for admin management operations
    public interface IAdminService
    {
        // Admin Registration
        Task<Result<AdminDto>> RegisterAdminAsync(RegisterAdminDto dto);

        // Admin Profile
        Task<Result<AdminDto>> GetAdminByIdAsync(Guid userId);
        Task<Result<PagedResult<AdminDto>>> GetAllAdminsAsync(int page = 1, int pageSize = 10);
        Task<Result<AdminDto>> UpdateAdminAsync(UpdateAdminDto dto);
        Task<Result> DeleteAdminAsync(Guid userId);

        // Admin Statistics
        Task<Result<AdminStatsDto>> GetAdminStatisticsAsync(Guid adminId);

        // Get admins by role
        Task<Result<PagedResult<AdminDto>>> GetAdminsByRoleAsync(string role, int page = 1, int pageSize = 10);

        // Get super admins only
        Task<Result<PagedResult<AdminDto>>> GetSuperAdminsAsync(int page = 1, int pageSize = 10);
    }

    /// DTO for admin statistics
    public class AdminStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalPatients { get; set; }
        public int TotalStudents { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalCases { get; set; }
        public int ActiveCases { get; set; }
        public int CompletedCases { get; set; }
        public int TotalSessions { get; set; }
    }
}
