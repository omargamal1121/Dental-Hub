namespace DentalHub.Application.DTOs.Students
{
    /// DTO for student information
    public class StudentDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public int UniversityId { get; set; }
        public int Level { get; set; }
        public DateTime CreateAt { get; set; }
        public int TotalRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int TotalSessions { get; set; }
    }
}
