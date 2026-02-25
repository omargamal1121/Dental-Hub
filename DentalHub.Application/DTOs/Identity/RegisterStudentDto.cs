using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Identity
{
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
        [MinLength(4, ErrorMessage = "Invalid University ID Must Be More than 4 char ")]
        public string UniversityId { get; set; } = null!;

        [Required(ErrorMessage = "UserName required")]
		[RegularExpression(@"^(?=.{3,20}$)(?!.*__)[a-z][a-z0-9_]*[a-z0-9]$",
	    ErrorMessage = "Invalid username format")]

        
        public string Username { get; set; } = null!;
        [Required(ErrorMessage ="Level is required")]
        [Range(1,7,ErrorMessage ="Level must be between 1 and 7")]

		public int Level { get; set; }
		[Required(ErrorMessage = "Phone is required")]
		[Phone(ErrorMessage = "Invalid phone number")]
		[RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone must be a valid Egyptian number")]
		public string Phone { get; set; }
	}
}
