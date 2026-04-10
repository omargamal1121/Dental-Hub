using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class DiagnosisFactory
    {
        public static Diagnosis Create(Guid patientCaseId, DiagnosisStage stage, Guid caseTypeId, string notes, Guid? createdById, string role, List<int>? teethNumbers = null)
        {
            return new Diagnosis
            {
                PatientCaseId = patientCaseId,
                Stage = stage,
                CaseTypeId = caseTypeId,
                Notes = notes,
                CreatedById = createdById,
                Role = role,
                TeethNumbers = teethNumbers ?? new List<int>()
            };
        }
    }
}
