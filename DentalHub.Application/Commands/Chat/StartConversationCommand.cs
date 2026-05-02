using System;
using MediatR;

namespace DentalHub.Application.Commands.Chat
{
    public class StartConversationCommand : IRequest<StartConversationCommandResponse>
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class StartConversationCommandResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? ConversationId { get; set; }
    }
}
