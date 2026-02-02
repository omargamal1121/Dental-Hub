namespace DentalHub.Application.DTOs.Sessions
{
    /// <summary>
    /// DTO for session information
    /// </summary>
    public class SessionDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public string TreatmentType { get; set; } = string.Empty;
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalNotes { get; set; }
        public int TotalMedia { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
