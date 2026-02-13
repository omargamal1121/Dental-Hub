using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Cases
{
    /// DTO for creating a new patient case
    public class CreateCaseDto
    {
        [Required(ErrorMessage = "Patient ID is required")]
        public Guid PatientId { get; set; }

		public Guid CaseTypeId { get; set; }


		[StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
        public string? Description { get; set; }
    }
}
