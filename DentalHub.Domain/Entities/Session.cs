namespace DentalHub.Domain.Entities
{
    public class Session : BaseEntitiy
	{
        public Guid CaseId { get; set; }
        public Guid StudentId { get; set; }
		public Student  Student { get; set; }
		public DateTime StartAt { get; set; }
		public DateTime EndAt { get; set; }
        public SessionStatus Status { get; set; }

        public PatientCase PatientCase { get; set; }
		public int Grade { get; set; }
		public string DoctorNote { get; set; }

		public Guid? EvaluteDoctorId { get; set; }
		public Doctor? EvaluteDoctor { get; set; }
		public ICollection<Media>  Medias { get; set; }

      //  public ICollection<SessionNote> SessionNotes { get; set; }
		public Patient  Patient { get; set; }
		public Guid  PatientId { get; set; }
	}
}
