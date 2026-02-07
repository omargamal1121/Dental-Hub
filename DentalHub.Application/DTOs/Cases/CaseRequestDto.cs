namespace DentalHub.Application.DTOs.Cases
{
    /// DTO for case request information
    public class CaseRequestDto
    {
        public Guid Id { get; set; }
        public Guid PatientCaseId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string TreatmentType { get; set; } = string.Empty;
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public int Level { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
    }
}
