using System;

namespace DentalHub.Domain.Entities
{
    public class SessionNote : BaseEntitiy
	{
        public Guid SessionId { get; set; }
        public string Note { get; set; }

        public Session Session { get; set; }

        // Navigation: media attached to this note (stored in Media table, not a raw URL)
        public ICollection<Media> Medias { get; set; } = new List<Media>();
    }
}
