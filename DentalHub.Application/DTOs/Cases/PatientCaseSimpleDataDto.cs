using DentalHub.Application.DTOs.Diagnoses;

namespace DentalHub.Application.DTOs.Cases
{
	public class PatientCaseSimpleDataDto
	{
		public Guid Id { get; set; }
		public CaseStatus Status { get; set; } 
        public DateTime CreateAt { get; set; }
		public string Name { get; set; }
		public Guid? UniversityId { get; set; }
		public List<DiagnosisSimpleDto> Diagnoses { get; set; } = new();
	}

}
