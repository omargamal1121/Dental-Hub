using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class PatientCaseFactory
    {
        public static PatientCase Create(Guid patientId, string treatmentType, CaseStatus status)
        {
            return new PatientCase
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                TreatmentType = treatmentType,
                Status = status,
                CreateAt = DateTime.UtcNow
            };
        }
    }
}