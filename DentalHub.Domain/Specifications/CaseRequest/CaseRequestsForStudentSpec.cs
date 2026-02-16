using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Specifications.CaseRequests
{
    public class CaseRequestsForStudentSpec : BaseSpecification<CaseRequest>
    {
        public CaseRequestsForStudentSpec(Guid studentId)
            : base(cr => cr.StudentId == studentId)
        {
            AddInclude(cr => cr.PatientCase);
            AddInclude(cr => cr.Doctor);
        }
    }
}
