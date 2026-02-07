using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;

namespace DentalHub.Application.Services.Doctors
{
    public interface IDoctorService
    {
        // Doctor Profile
        Task<Result<DoctorDto>> GetDoctorByIdAsync(Guid userId);
        Task<Result<List<DoctorDto>>> GetAllDoctorsAsync(int page = 1, int pageSize = 10);
        Task<Result<DoctorDto>> UpdateDoctorAsync(UpdateDoctorDto dto);
        Task<Result> DeleteDoctorAsync(Guid userId);

        // Doctor Statistics
        Task<Result<DoctorStatsDto>> GetDoctorStatisticsAsync(Guid doctorId);

        // Get doctors by university
        Task<Result<List<DoctorDto>>> GetDoctorsByUniversityAsync(
            int universityId, int page = 1, int pageSize = 10);
    }

    /// DTO for doctor statistics
    public class DoctorStatsDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int TotalStudents { get; set; }
        public int ActiveCases { get; set; }
        public int CompletedCases { get; set; }
    }
}
