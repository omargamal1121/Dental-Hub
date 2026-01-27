namespace DentalHub.Domain.Entities
{
    public class Doctor
    {
        public Guid UserId { get; set; }
        public string Specialty { get; set; }
		public int UniversityId { get; set; }

		public User User { get; set; }
		public List<CaseRequest>  CaseRequests { get; set; }
	}
}
