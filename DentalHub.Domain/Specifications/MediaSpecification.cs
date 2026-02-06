using DentalHub.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace DentalHub.Domain.Specifications
{
    public static class MediaSpecification
    {
        public static Expression<Func<Media, bool>> ByPatientId(Guid patientId)
        {
            return m => m.PatientId == patientId;
        }

        public static Expression<Func<Media, bool>> BySessionId(Guid sessionId)
        {
            return m => m.SessionId == sessionId;
        }
    }
}