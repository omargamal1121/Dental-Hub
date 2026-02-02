using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Cases
{
    /// <summary>
    /// DTO for creating a case request
    /// الطالب بيعمل طلب للدكتور
    /// </summary>
    public class CreateCaseRequestDto
    {
        [Required(ErrorMessage = "Patient case ID is required")]
        public Guid PatientCaseId { get; set; }

        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Doctor ID is required")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;
    }
}
