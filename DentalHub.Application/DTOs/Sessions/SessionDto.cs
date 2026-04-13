namespace DentalHub.Application.DTOs.Sessions
{
    /// DTO for session information
    public class SessionDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public string TreatmentType { get; set; } = string.Empty;
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;

        /// <summary>Session start date and time (maps to Session.StartAt)</summary>
        public DateTime ScheduledAt { get; set; }

        /// <summary>Session end date and time (maps to Session.EndAt)</summary>
        public DateTime EndAt { get; set; }

        public string Status { get; set; } = string.Empty;
        public int TotalNotes { get; set; }
        public int TotalMedia { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
