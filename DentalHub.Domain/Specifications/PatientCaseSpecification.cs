using DentalHub.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace DentalHub.Domain.Specifications
{
    public static class PatientCaseSpecification
    {
        public static Expression<Func<PatientCase, bool>> ByPatientId(Guid patientId)
        {
            return pc => pc.PatientId == patientId;
        }

        public static Expression<Func<PatientCase, bool>> ByStatus(CaseStatus status)
        {
            return pc => pc.Status == status;
        }
    }
}