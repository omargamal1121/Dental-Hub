using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Cases
{
    /// DTO for creating a new patient case
    public class CreateCaseDto
    {
        [Required(ErrorMessage = "Patient public ID is required")]
        public string PatientId { get; set; }

        [Required(ErrorMessage = "Case type public ID is required")]
		public string CaseTypeId { get; set; }


		[StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
        public string? Description { get; set; }
    }
}
