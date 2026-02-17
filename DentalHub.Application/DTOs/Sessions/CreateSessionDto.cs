using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Sessions
{
    /// DTO for creating a new session
    public class CreateSessionDto
    {
        [Required(ErrorMessage = "Case public ID is required")]
        public string CaseId { get; set; }

        [Required(ErrorMessage = "Student public ID is required")]
        public string StudentId { get; set; }

        [Required(ErrorMessage = "Patient public ID is required")]
        public string PatientId { get; set; }

        [Required(ErrorMessage = "Scheduled date is required")]
        public DateTime ScheduledAt { get; set; }
    }
}
