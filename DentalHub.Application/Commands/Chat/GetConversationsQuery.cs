using System;
using System.Collections.Generic;
using MediatR;
using DentalHub.Application.DTOs.Chat;

namespace DentalHub.Application.Commands.Chat
{
    public class GetConversationsQuery : IRequest<GetConversationsQueryResponse>
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class GetConversationsQueryResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ConversationDto> Conversations { get; set; } = new();
    }
}
