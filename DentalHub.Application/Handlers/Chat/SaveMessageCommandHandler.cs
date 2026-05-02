using System.Threading;
using System.Threading.Tasks;
using DentalHub.Application.Commands.Chat;
using DentalHub.Application.Interfaces;
using MediatR;

namespace DentalHub.Application.Handlers.Chat
{
    public class SaveMessageCommandHandler : IRequestHandler<SaveMessageCommand, SaveMessageCommandResponse>
    {
        private readonly IChatService _chatService;

        public SaveMessageCommandHandler(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<SaveMessageCommandResponse> Handle(SaveMessageCommand request, CancellationToken cancellationToken)
        {
            return await _chatService.SaveMessageAsync(request);
        }
    }
}
