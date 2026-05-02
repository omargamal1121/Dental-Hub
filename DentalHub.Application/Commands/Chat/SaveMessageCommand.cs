using System;
using MediatR;

namespace DentalHub.Application.Commands.Chat
{
    public class SaveMessageCommand : IRequest<SaveMessageCommandResponse>
    {
        public string UserId { get; set; } = string.Empty; 
        public Guid ConversationId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsAiMessage { get; set; } 
    }

    public class SaveMessageCommandResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
