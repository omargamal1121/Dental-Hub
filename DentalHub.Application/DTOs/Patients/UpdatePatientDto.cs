using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Patients
{
    /// <summary>
    /// DTO for updating patient information
    /// </summary>
    public class UpdatePatientDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\u0621-\u064A\s]+$", ErrorMessage = "Full name can only contain letters and spaces")]
        public string? FullName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone must be a valid Egyptian number")]
        public string? Phone { get; set; }

        [Range(5, 120, ErrorMessage = "Age must be between 5 and 120")]
        public int? Age { get; set; }
    }
}
