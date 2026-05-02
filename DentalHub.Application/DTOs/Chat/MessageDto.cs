using System;

namespace DentalHub.Application.DTOs.Chat
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public string Sender { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
