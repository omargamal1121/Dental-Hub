namespace DentalHub.Application.DTOs.Students
{
    /// DTO for updating student information
    public class UpdateStudentDto
    {
        public Guid PublicId { get; set; }

        public string? FullName { get; set; }

        public string? University { get; set; }

        public int? Level { get; set; }
    }
}
