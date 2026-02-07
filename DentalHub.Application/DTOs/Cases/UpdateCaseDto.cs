using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Cases
{
    /// DTO for updating a patient case
    public class UpdateCaseDto
    {
        [Required]
        public Guid Id { get; set; }

        [StringLength(200, MinimumLength = 3)]
        public string? TreatmentType { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
