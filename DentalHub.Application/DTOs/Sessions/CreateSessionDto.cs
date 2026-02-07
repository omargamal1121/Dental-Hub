using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Sessions
{
    /// DTO for creating a new session
    public class CreateSessionDto
    {
        [Required(ErrorMessage = "Case ID is required")]
        public Guid CaseId { get; set; }

        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "Scheduled date is required")]
        public DateTime ScheduledAt { get; set; }
    }
}
