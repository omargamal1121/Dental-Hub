namespace DentalHub.Application.DTOs.Sessions
{
    /// DTO for session information
    public class SessionDto
    {
        public string Id { get; set; }
        public string CaseId { get; set; }
        public string TreatmentType { get; set; } = string.Empty;
        public string PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalNotes { get; set; }
        public int TotalMedia { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
