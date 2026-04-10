namespace DentalHub.Domain.Entities
{
	public class Diagnosis : BaseEntitiy
	{

		public Guid PatientCaseId { get; set; }
		public PatientCase PatientCase { get; set; }

		public DiagnosisStage Stage { get; set; } 

		public Guid CaseTypeId { get; set; }
		public CaseType CaseType { get; set; }

		public string Notes { get; set; }

		public Guid? CreatedById { get; set; }
		public User? User { get; set; }
		public string Role { get; set; }

		public bool? IsAccepted { get; set; }
		public List<int> TeethNumbers { get; set; } = new List<int>();
	}
}
