using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Specifications.PatientCases
{
    public class PatientCasesForPatientSpec : BaseSpecification<PatientCase>
    {
        public PatientCasesForPatientSpec(Guid patientId)
            : base(pc => pc.PatientId == patientId)
        {
            AddInclude(pc => pc.CaseRequests);
            AddInclude(pc => pc.Sessions);
        }
    }
}