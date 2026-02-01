using System;

namespace DentalHub.Domain.Entities
{
    public class CaseRequest:BaseEntitiy
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string Description { get; set; }
        public RequestStatus Status { get; set; }

		public Guid PatientCaseId { get; set; }
		public PatientCase PatientCase { get; set; }
        public Student Student { get; set; }

		public Guid DoctorId { get; set; }
		public Doctor Doctor { get; set; }
	}
}
