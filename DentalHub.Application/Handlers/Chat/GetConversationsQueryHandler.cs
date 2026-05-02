using System.Threading;
using System.Threading.Tasks;
using DentalHub.Application.Commands.Chat;
using DentalHub.Application.Interfaces;
using MediatR;

namespace DentalHub.Application.Handlers.Chat
{
    public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, GetConversationsQueryResponse>
    {
        private readonly IChatService _chatService;

        public GetConversationsQueryHandler(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<GetConversationsQueryResponse> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
        {
            return await _chatService.GetConversationsAsync(request);
        }
    }
}
