using DentalHub.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace DentalHub.Domain.Specifications
{
    public static class SessionSpecification
    {
        public static Expression<Func<Session, bool>> ByCaseId(Guid caseId)
        {
            return s => s.CaseId == caseId;
        }

        public static Expression<Func<Session, bool>> ByStudentId(Guid studentId)
        {
            return s => s.StudentId == studentId;
        }

        public static Expression<Func<Session, bool>> ByStatus(SessionStatus status)
        {
            return s => s.Status == status;
        }
    }
}