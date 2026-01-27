using System;

namespace DentalHub.Domain.Entities
{
    public class Media
    {
        public Guid Id { get; set; }
        public Guid? SessionId { get; set; }
		public Patient  Patient { get; set; }
		public Guid  PatientId { get; set; }
		public string MediaUrl { get; set; }

        public Session? Session { get; set; }
    }
}

