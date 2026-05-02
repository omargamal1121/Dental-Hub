using System.Threading;
using System.Threading.Tasks;
using DentalHub.Application.Commands.Chat;
using DentalHub.Application.Interfaces;
using MediatR;

namespace DentalHub.Application.Handlers.Chat
{
    public class StartConversationCommandHandler : IRequestHandler<StartConversationCommand, StartConversationCommandResponse>
    {
        private readonly IChatService _chatService;

        public StartConversationCommandHandler(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<StartConversationCommandResponse> Handle(StartConversationCommand request, CancellationToken cancellationToken)
        {
            return await _chatService.StartConversationAsync(request);
        }
    }
}
