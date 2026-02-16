namespace DentalHub.Domain.Entities
{
    public class PatientCase : BaseEntitiy
	{
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public Guid PatientId { get; set; }
		public Guid CaseTypeId { get; set; }

		public CaseStatus Status { get; set; }
		public string Description { get; set; }
		public CaseType CaseType { get; set; } = null!;
		public Patient Patient { get; set; } = null!;
		public ICollection<CaseRequest> CaseRequests { get; set; } = new List<CaseRequest>();
		public ICollection<Session> Sessions { get; set; } = new List<Session>();
		public ICollection<Media> Medias { get; set; } = new List<Media>();

	}
}
