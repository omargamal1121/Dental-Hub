using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Students
{
    /// <summary>
    /// DTO for updating student information
    /// </summary>
    public class UpdateStudentDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\u0621-\u064A\s]+$", ErrorMessage = "Full name can only contain letters and spaces")]
        public string? FullName { get; set; }

        [StringLength(200, MinimumLength = 3, ErrorMessage = "University name must be between 3 and 200 characters")]
        public string? University { get; set; }

        [Range(1, 5, ErrorMessage = "Level must be between 1 and 5")]
        public int? Level { get; set; }
    }
}
