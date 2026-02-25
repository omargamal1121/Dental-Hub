using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.DTOs.Students;

namespace DentalHub.Application.Services.Students
{
    public interface IStudentService
    {
        // Student Profile
        Task<Result<StudentDto>> GetStudentByPublicIdAsync(string publicId);

        // For the student himself - searches by UserId (coming from the JWT token)
        Task<Result<StudentDto>> GetStudentByUserIdAsync(string userId);

        Task<Result<PagedResult<StudentDto>>> GetAllStudentsAsync(int page = 1, int pageSize = 10);
        Task<Result<StudentDto>> UpdateStudentAsync(UpdateStudentDto dto);
        Task<Result> DeleteStudentByPublicIdAsync(string publicId);


        Task<Result<PagedResult<PatientCaseDto>>> GetMyCasesForStudentAsync(
            string studentPublicId, string? casetype = null, int page = 1, int pageSize = 10);

 
        Task<Result<PagedResult<PatientCaseDto>>> GetAvailableCasesForStudentAsync(
            string studentPublicId,string? casetype=null, int page = 1, int pageSize = 10);

        // Student Statistics
        Task<Result<StudentStatsDto>> GetStudentStatisticsAsync(string studentPublicId);
    }

    /// DTO for student statistics
    public class StudentStatsDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public int TotalCases { get; set; }
    }
}
