using System;
using System.Collections.Generic;


namespace DentalHub.Domain.Entities
{
    public class PatientCase
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string TreatmentType { get; set; }
        public CaseStatus Status { get; set; }

        public Patient Patient { get; set; }

        // One-to-Many
        public ICollection<CaseRequest> CaseRequests { get; set; }
        public ICollection<Session> Sessions { get; set; }
    }
}
