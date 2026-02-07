using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class CaseRequestFactory
    {
        public static CaseRequest Create(Guid studentId, Guid doctorId, Guid patientCaseId, string description, RequestStatus status)
        {

            if (studentId == Guid.Empty)
                throw new ArgumentException("StudentId cannot be empty");

            if (doctorId == Guid.Empty)
                throw new ArgumentException("DoctorId cannot be empty");

            if (patientCaseId == Guid.Empty)
                throw new ArgumentException("PatientCaseId cannot be empty");

            if (string.IsNullOrWhiteSpace(description))
                description = "No description provided";

            if (!Enum.IsDefined(typeof(RequestStatus), status))
                status = RequestStatus.Pending;
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