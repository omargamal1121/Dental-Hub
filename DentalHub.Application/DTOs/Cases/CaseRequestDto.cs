using DentalHub.Application.DTOs.CaseTypes;

namespace DentalHub.Application.DTOs.Cases
{
    /// DTO for case request information
    public class CaseRequestDto
    {
        public string Id { get; set; }
        public string PatientCaseId { get; set; }
        public string PatientName { get; set; } = string.Empty;
		public string CaseName { get; set; }
		public string StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public int Level { get; set; }
        public string DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
    }
  
}
