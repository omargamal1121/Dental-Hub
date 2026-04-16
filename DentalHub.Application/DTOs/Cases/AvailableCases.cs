using DentalHub.Application.DTOs.CaseTypes;

namespace DentalHub.Application.DTOs.Cases
{
	public class AvailableCasesDto
    {
		public Guid Id { get; set; }
		public Guid PatientId { get; set; }
		public string PatientName { get; set; } = string.Empty;
		public int PatientAge { get; set; }
		public CaseTypeDto CaseType { get; set; }
		public string Status { get; set; } = string.Empty;
		public DateTime CreateAt { get; set; }
		public Gender gender { get; set; }
        public List<Diagnosisdto> Diagnosisdto { get; set; }



        public List<string> ImageUrls { get; set; } = new List<string>();

	}

}
