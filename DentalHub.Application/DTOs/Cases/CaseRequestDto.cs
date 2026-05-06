using DentalHub.Application.DTOs.CaseTypes;
using DentalHub.Application.DTOs.Diagnoses;

namespace DentalHub.Application.DTOs.Cases
{
    /// DTO for case request information
    public class CaseRequestDto
    {
        public Guid Id { get; set; }
        public Guid PatientCasePublicId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        public List<DiagnosisDto> Diagnosisdto { get; set; } = new();
		public Guid StudentPublicId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public int Level { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public bool IsRejectedStudent { get; set; }
        public List<string> ImageUrls { get; set; } = new();


    }
  
}
