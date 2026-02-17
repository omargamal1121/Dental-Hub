namespace DentalHub.Application.DTOs.Cases
{
	public class PatientCaseSimpleDataDto
	{
		public string Id { get; set; }
		public CaseStatus Status { get; set; } 
        public DateTime CreateAt { get; set; }
		public string Name { get; set; }
	}

}
