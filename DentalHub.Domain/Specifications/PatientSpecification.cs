using DentalHub.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace DentalHub.Domain.Specifications
{
    public static class PatientSpecification
    {
        public static Expression<Func<Patient, bool>> ById(Guid patientId)
        {
            return p => p.UserId == patientId;
        }
    }
}