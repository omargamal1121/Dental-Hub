using System;

namespace DentalHub.Domain.Entities
{
    public class CaseRequest
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public Guid StudentId { get; set; }
        public string Description { get; set; }
        public RequestStatus Status { get; set; }

        public PatientCase PatientCase { get; set; }
        public Student Student { get; set; }
    }
}
