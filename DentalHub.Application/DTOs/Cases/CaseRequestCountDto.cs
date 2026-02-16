namespace DentalHub.Application.DTOs.Cases
{
	public class CaseRequestCountDto
    {
		public Guid Id { get; set; }
		public CaseStatus  Status { get; set; }
        public Guid doctorid   { get; set; }
}
}
