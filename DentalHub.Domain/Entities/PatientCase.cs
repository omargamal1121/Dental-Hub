using System.Collections;

namespace DentalHub.Domain.Entities
{
    public class PatientCase : BaseEntitiy
	{
        public Guid PatientId { get; set; }
		//public Guid CaseTypeId { get; set; }

		public CaseStatus Status { get; set; }
		public string Description { get; set; }=null!;
		//public CaseType CaseType { get; set; } = null!;
		public Guid? AssignedStudentId { get; set; }
		public Student? AssignedStudent { get; set; }

		public Guid? AssignedDoctorId { get; set; }
		public Doctor? AssignedDoctor { get; set; }
		public ICollection<Diagnosis> Diagnosiss { get; set; }
		public Patient Patient { get; set; } = null!;
		public ICollection<CaseRequest> CaseRequests { get; set; } = new List<CaseRequest>();
		public ICollection<Session> Sessions { get; set; } = new List<Session>();
		public ICollection<Media> Medias { get; set; } = new List<Media>();

		public bool IsPublic { get; set; } = false;
		public Guid? UniversityId { get; set; }
		public University? University { get; set; }

        public Guid? CreatedById { get; set; }
        public string? CreatedByRole { get; set; }
	}
}
