using System;

namespace DentalHub.Domain.Entities
{
    public class Message : BaseEntitiy
    {
        public Guid ConversationId { get; set; }
        public string Sender { get; set; } = string.Empty; 
        public string Content { get; set; } = string.Empty;
        
        public Conversation Conversation { get; set; } = null!;
    }
}
