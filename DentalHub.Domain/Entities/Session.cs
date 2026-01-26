using System;
using System.Collections.Generic;

namespace DentalHub.Domain.Entities
{
    public class Session
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public Guid StudentId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public SessionStatus Status { get; set; }

        public PatientCase PatientCase { get; set; }

        public ICollection<SessionMedia> SessionMedias { get; set; }
        public ICollection<SessionNote> SessionNotes { get; set; }
    }
}
