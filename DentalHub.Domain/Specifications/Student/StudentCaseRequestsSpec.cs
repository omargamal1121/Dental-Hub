using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Specifications.Students
{
    public class StudentCaseRequestsSpec : BaseSpecification<CaseRequest>
    {
        public StudentCaseRequestsSpec(Guid studentId)
            : base(cr => cr.StudentId == studentId)
        {
            AddInclude(cr => cr.PatientCase);
            AddInclude(cr => cr.Doctor);
        }
    }
}
