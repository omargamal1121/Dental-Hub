namespace DentalHub.Application.DTOs.Students
{
    /// DTO for student information
    public class StudentDto
    {
        public Guid PublicId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public Guid UniversityId { get; set; }
        public int Level { get; set; }
        public DateTime CreateAt { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;

    }
}