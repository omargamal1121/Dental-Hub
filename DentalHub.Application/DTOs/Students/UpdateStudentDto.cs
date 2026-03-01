namespace DentalHub.Application.DTOs.Students
{
    /// DTO for updating student information
    public class UpdateStudentDto
    {
        public string PublicId { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public string? University { get; set; }

        public int? Level { get; set; }
    }
}
