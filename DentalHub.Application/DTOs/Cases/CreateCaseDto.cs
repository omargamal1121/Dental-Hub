using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Cases
{
    /// DTO for creating a new patient case
    public class CreateCaseDto
    {
        [Required(ErrorMessage = "Patient ID is required")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Treatment type is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Treatment type must be between 3 and 200 characters")]
        public string TreatmentType { get; set; } = string.Empty;

        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
        public string? Description { get; set; }
    }
}
