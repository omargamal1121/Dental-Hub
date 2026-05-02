using System.Threading;
using System.Threading.Tasks;
using DentalHub.Application.Commands.Chat;
using DentalHub.Application.Interfaces;
using MediatR;

namespace DentalHub.Application.Handlers.Chat
{
    public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, GetMessagesQueryResponse>
    {
        private readonly IChatService _chatService;

        public GetMessagesQueryHandler(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<GetMessagesQueryResponse> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            return await _chatService.GetMessagesAsync(request);
        }
    }
}
