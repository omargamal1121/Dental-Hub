using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class SessionNoteFactory
    {
        public static SessionNote Create(Guid sessionId, string note)
        {
            return new SessionNote
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                Note = note,
                CreateAt = DateTime.UtcNow
            };
        }
    }
}