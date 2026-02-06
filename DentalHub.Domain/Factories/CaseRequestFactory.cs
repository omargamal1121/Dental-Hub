using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class CaseRequestFactory
    {
        public static CaseRequest Create(Guid studentId, Guid doctorId, Guid patientCaseId, string description, RequestStatus status)
        {
            return new CaseRequest
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                DoctorId = doctorId,
                PatientCaseId = patientCaseId,
                Description = description,
                Status = status,
                CreateAt = DateTime.UtcNow
            };
        }
    }
}