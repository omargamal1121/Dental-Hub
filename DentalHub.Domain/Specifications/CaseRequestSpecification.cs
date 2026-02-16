using DentalHub.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace DentalHub.Domain.Specifications
{
    public static class CaseRequestSpecification
    {
        public static Expression<Func<CaseRequest, bool>> ByStudentId(Guid studentId)
        {
            return cr => cr.StudentId == studentId;
        }

        public static Expression<Func<CaseRequest, bool>> ByDoctorId(Guid doctorId)
        {
            return cr => cr.DoctorId == doctorId;
        }

        public static Expression<Func<CaseRequest, bool>> ByStatus(RequestStatus status)
        {
            return cr => cr.Status == status;
        }
    }
}