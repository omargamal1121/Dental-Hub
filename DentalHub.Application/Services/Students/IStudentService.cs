using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.DTOs.Students;

namespace DentalHub.Application.Services.Students
{
    public interface IStudentService
    {
        // Student Profile
        Task<Result<StudentDto>> GetStudentByIdAsync(Guid userId);
        Task<Result<List<StudentDto>>> GetAllStudentsAsync(int page = 1, int pageSize = 10);
        Task<Result<StudentDto>> UpdateStudentAsync(UpdateStudentDto dto);
        Task<Result> DeleteStudentAsync(Guid userId);

        // Available Cases for Student
        Task<Result<List<PatientCaseDto>>> GetAvailableCasesForStudentAsync(
            Guid studentId, int page = 1, int pageSize = 10);

        // Student Statistics
        Task<Result<StudentStatsDto>> GetStudentStatisticsAsync(Guid studentId);
    }

    /// <summary>
    /// DTO for student statistics
    /// </summary>
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
