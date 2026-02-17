using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Cases
{
    /// DTO for creating a case request
    public class CreateCaseRequestDto
    {
        [Required(ErrorMessage = "Patient case public ID is required")]
        public string PatientCasePublicId { get; set; }

        [Required(ErrorMessage = "Student public ID is required")]
        public string StudentPublicId { get; set; }

        [Required(ErrorMessage = "Doctor public ID is required")]
        public string DoctorPublicId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;
    }
}
