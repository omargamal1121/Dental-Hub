using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Specifications.Students
{
    public class StudentDashboardSpec : BaseSpecification<PatientCase>
    {
        public StudentDashboardSpec(Guid studentId)
            : base(pc => pc.StudentId == studentId && pc.Status == CaseStatus.InProgress)
        {
            AddInclude(pc => pc.Sessions);
            AddInclude(pc => pc.CaseRequests);
        }
    }
}