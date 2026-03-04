namespace DentalHub.Application.DTOs.Sessions
{
    public class CreateSessionDto
    {
        public Guid CaseId { get; set; } 

        public Guid StudentId { get; set; } 

        public Guid PatientId { get; set; } 

        public DateTime ScheduledAt { get; set; }
    }
}
