using System;

namespace DentalHub.Domain.Entities
{
    public class SessionMedia
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public string MediaUrl { get; set; }

        public Session Session { get; set; }
    }
}

