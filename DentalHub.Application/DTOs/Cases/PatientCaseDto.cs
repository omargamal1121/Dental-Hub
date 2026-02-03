namespace DentalHub.Application.DTOs.Cases
{
    /// <summary>
    /// DTO for patient case information
    /// معلومات الحالة
    /// </summary>
    public class PatientCaseDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public string TreatmentType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public int TotalSessions { get; set; }
        public int PendingRequests { get; set; }
    }
}
