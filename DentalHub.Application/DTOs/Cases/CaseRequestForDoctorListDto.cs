namespace DentalHub.Application.DTOs.Cases
{
	public class CaseRequestForDoctorListDto
	{
		public Guid RequestId { get; set; }
		public Guid StudentId { get; set; }
		public Guid PatientId { get; set; }
		public Guid PatientCaseId { get; set; }
		public string StudentName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty; 
		public string Status { get; set; } = string.Empty; 
		public DateTime CreatedAt { get; set; }
		public bool Seen { get; set; }
		public string CaseType { get; set; }
	}
}
