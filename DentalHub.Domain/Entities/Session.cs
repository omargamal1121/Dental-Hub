namespace DentalHub.Domain.Entities
{
    public class Session : BaseEntitiy
	{
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public Guid CaseId { get; set; }
        public Guid StudentId { get; set; }
		public Student  Student { get; set; }
		public DateTime ScheduledAt { get; set; }
        public SessionStatus Status { get; set; }

        public PatientCase PatientCase { get; set; }

        public ICollection<Media>  Medias { get; set; }
        public ICollection<SessionNote> SessionNotes { get; set; }
		public Patient  Patient { get; set; }
		public Guid  PatientId { get; set; }
	}
}
