namespace DentalHub.Domain.Entities
{
    public class PatientCase : BaseEntitiy
	{
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string TreatmentType { get; set; }
        public CaseStatus Status { get; set; }

		public Patient Patient { get; set; }

        public ICollection<CaseRequest> CaseRequests { get; set; }
        public ICollection<Session> Sessions { get; set; }
        public ICollection<Media>  Medias { get; set; }
	}
}
