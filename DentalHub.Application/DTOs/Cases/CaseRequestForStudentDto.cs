using DentalHub.Application.DTOs.CaseTypes;

namespace DentalHub.Application.DTOs.Cases
{
	public class CaseRequestForStudentDto
	{
		public Guid RequestId { get; set; }
		public Guid PatientCaseId { get; set; }
	


		public Guid DoctorId { get; set; }
		public  string DoctorName { get; set; }
		public string CaseType { get; set; }

		public string Description { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public bool Seen { get; set; }
	}

}
