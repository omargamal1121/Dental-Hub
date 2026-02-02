using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Identity
{
    /// <summary>
    /// DTO for student registration
    /// </summary>
    public class RegisterStudentDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\u0621-\u064A\s]+$", ErrorMessage = "Full name can only contain letters and spaces")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "University is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "University name must be between 3 and 200 characters")]
        public string University { get; set; } = string.Empty;

        [Required(ErrorMessage = "University ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "University ID must be a positive number")]
        public int UniversityId { get; set; }

        [Required(ErrorMessage = "Level is required")]
        [Range(1, 5, ErrorMessage = "Level must be between 1 and 5")]
        public int Level { get; set; }
    }
}
