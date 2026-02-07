using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class PatientCaseFactory
    {
        public static PatientCase Create(Guid patientId, string treatmentType, CaseStatus status)
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("PatientId cannot be empty");

            if (string.IsNullOrWhiteSpace(treatmentType))
                treatmentType = "General Treatment";

            if (!Enum.IsDefined(typeof(CaseStatus), status))
                status = CaseStatus.Pending;

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