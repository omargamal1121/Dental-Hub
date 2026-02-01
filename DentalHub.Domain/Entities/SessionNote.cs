using System;

namespace DentalHub.Domain.Entities
{
    public class SessionNote : BaseEntitiy
	{
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public string Note { get; set; }

        public Session Session { get; set; }
    }
}
