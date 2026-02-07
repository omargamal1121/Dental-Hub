using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class SessionFactory
    {
        public static Session Create(Guid caseId, Guid studentId, DateTime scheduledAt, SessionStatus status)
        {
            if (caseId == Guid.Empty)
                throw new ArgumentException("CaseId cannot be empty");

            if (studentId == Guid.Empty)
                throw new ArgumentException("StudentId cannot be empty");

            if (scheduledAt < DateTime.UtcNow)
                throw new ArgumentException("ScheduledAt must be a future date");

            return new Session
            {
                Id = Guid.NewGuid(),
                CaseId = caseId,
                StudentId = studentId,
                ScheduledAt = scheduledAt,
                Status = status,
                CreateAt = DateTime.UtcNow
            };
        }
    }
}