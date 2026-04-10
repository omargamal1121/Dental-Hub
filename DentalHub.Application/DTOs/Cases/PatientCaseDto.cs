using DentalHub.Application.DTOs.CaseTypes;
using DentalHub.Domain.Entities;

namespace DentalHub.Application.DTOs.Cases
{
    /// DTO for patient case information
    public class PatientCaseDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public Guid? UniversityId { get; set; }
        public string? UniversityName { get; set; }
        public DateTime CreateAt { get; set; }
        public int TotalSessions { get; set; }
        public int PendingRequests { get; set; }
		public Diagnosisdto  ?Diagnosisdto { get; set; }

		public List<string> ImageUrls { get; set; } = new List<string>();
    }
    public class Diagnosisdto
    {
		public Guid Id { get; set; }
		
		public string DiagnosisStage { get; set; }
		public string CaseType { get; set; }
		public string Notes { get; set; }
		public List<int> TeethNumbers { get; set; } = new List<int>();

	}
}
