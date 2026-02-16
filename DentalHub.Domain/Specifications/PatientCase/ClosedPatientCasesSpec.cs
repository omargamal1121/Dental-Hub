using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Specifications.PatientCases
{
    public class ClosedPatientCasesSpec : BaseSpecification<PatientCase>
    {
        public ClosedPatientCasesSpec(Guid patientId)
            : base(pc => pc.PatientId == patientId && pc.Status == CaseStatus.Completed)
        {
            AddInclude(pc => pc.Sessions);
            AddInclude(pc => pc.CaseRequests);
        }
    }
}
