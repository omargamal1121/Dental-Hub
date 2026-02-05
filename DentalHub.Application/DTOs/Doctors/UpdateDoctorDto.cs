using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Doctors
{
    /// <summary>
    /// DTO for updating doctor information
    /// </summary>
    public class UpdateDoctorDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\u0621-\u064A\s]+$", ErrorMessage = "Full name can only contain letters and spaces")]
        public string? FullName { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        public string? Name { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Specialty must be between 3 and 100 characters")]
        public string? Specialty { get; set; }

		public int UniversityId { get; set; }
	}
}
