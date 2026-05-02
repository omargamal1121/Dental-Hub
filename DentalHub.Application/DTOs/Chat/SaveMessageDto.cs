using System;

namespace DentalHub.Application.DTOs.Chat
{
    public class SaveMessageDto
    {
        public Guid ConversationId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsAiMessage { get; set; }
    }
}
