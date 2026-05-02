using System;
using System.Collections.Generic;
using MediatR;
using DentalHub.Application.DTOs.Chat;

namespace DentalHub.Application.Commands.Chat
{
    public class GetMessagesQuery : IRequest<GetMessagesQueryResponse>
    {
        public string UserId { get; set; } = string.Empty;
        public Guid ConversationId { get; set; }
    }

    public class GetMessagesQueryResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<MessageDto> Messages { get; set; } = new();
    }
}
