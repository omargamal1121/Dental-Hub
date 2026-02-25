using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Identity
{

    public class RegisterPatientDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\u0621-\u064A\s]+$", ErrorMessage = "Full name can only contain letters and spaces")]
        public string FullName { get; set; } = string.Empty;

    
      
        public string Email { get; set; } 

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone must be a valid Egyptian number")]
        public string Phone { get; set; } = string.Empty;

		public Gender  Gender { get; set; }
        [Required(ErrorMessage = "National ID is required")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits")]
		public string NationalId { get; set; } = null!;


        [Required(ErrorMessage = "Birth date is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format")]

        
		public DateTime BirthDate { get; set; }
	}
}
