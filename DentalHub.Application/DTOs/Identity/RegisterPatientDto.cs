using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Identity
{
    /// DTO for patient registration
    public class RegisterPatientDto
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

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone must be a valid Egyptian number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required")]
        [Range(5, 120, ErrorMessage = "Age must be between 5 and 120")]
        public int Age { get; set; }
		public Gender  Gender { get; set; }
		public string NationalId { get; set; }
		public DateTime BirthDate { get; set; }
	}
}
