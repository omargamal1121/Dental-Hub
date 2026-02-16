using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Specifications.CaseRequests
{
    public class PendingCaseRequestsForDoctorSpec : BaseSpecification<CaseRequest>
    {
        public PendingCaseRequestsForDoctorSpec(Guid doctorId)
            : base(cr => cr.DoctorId == doctorId && cr.Status == RequestStatus.Pending)
        {
            AddInclude(cr => cr.PatientCase);
            AddInclude(cr => cr.Student);
        }
    }
}