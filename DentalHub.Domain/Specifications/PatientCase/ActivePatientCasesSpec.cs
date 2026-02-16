using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Specifications.PatientCases
{
    public class ActivePatientCasesSpec : BaseSpecification<PatientCase>
    {
        public ActivePatientCasesSpec(Guid patientId)
            : base(pc => pc.PatientId == patientId && pc.Status == CaseStatus.InProgress)
        {
            AddInclude(pc => pc.Sessions);
            AddInclude(pc => pc.CaseRequests);
        }
    }
}