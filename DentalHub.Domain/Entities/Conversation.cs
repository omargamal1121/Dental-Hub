using System;
using System.Collections.Generic;

namespace DentalHub.Domain.Entities
{
    public class Conversation : BaseEntitiy
    {
        public string UserId { get; set; } = string.Empty;
        
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
