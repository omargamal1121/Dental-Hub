using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class SessionFactory
    {
        public static Session Create(Guid caseId, Guid studentId, DateTime scheduledAt, SessionStatus status)
        {
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