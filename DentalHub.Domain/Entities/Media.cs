using System;

namespace DentalHub.Domain.Entities
{
    public class Media: BaseEntitiy
	{
        public Guid? SessionId { get; set; }
		public Patient  Patient { get; set; }
		public Guid  PatientId { get; set; }
		public string MediaUrl { get; set; } = null!;

        public Session? Session { get; set; }
		public Guid? CaseTypeId { get; set; }
		public CaseType?  CaseType { get; set; }
	}
}

