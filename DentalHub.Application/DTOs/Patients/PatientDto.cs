 namespace DentalHub.Application.DTOs.Patients
{
    /// DTO for patient information
    public class PatientDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int Age { get; set; }
        public DateTime CreateAt { get; set; }
        public int TotalCases { get; set; }
        public int ActiveCases { get; set; }
    }
}
