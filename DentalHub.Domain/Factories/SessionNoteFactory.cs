using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class SessionNoteFactory
    {
        public static SessionNote Create(Guid sessionId, string note)
        {
            if (sessionId == Guid.Empty)
                throw new ArgumentException("SessionId cannot be empty");

            if (string.IsNullOrWhiteSpace(note))
                note = "No notes provided";

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