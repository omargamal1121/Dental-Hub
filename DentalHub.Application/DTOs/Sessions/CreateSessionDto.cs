namespace DentalHub.Application.DTOs.Sessions
{
    public class CreateSessionDto
    {
        public string CaseId { get; set; } = string.Empty;

        public string StudentId { get; set; } = string.Empty;

        public string PatientId { get; set; } = string.Empty;

        public DateTime ScheduledAt { get; set; }
    }
}
