using System.Threading.Tasks;
using DentalHub.Application.Commands.Chat;

namespace DentalHub.Application.Interfaces
{
    public interface IChatService
    {
        Task<StartConversationCommandResponse> StartConversationAsync(StartConversationCommand command);
        Task<SaveMessageCommandResponse> SaveMessageAsync(SaveMessageCommand command);
        Task<GetConversationsQueryResponse> GetConversationsAsync(GetConversationsQuery query);
        Task<GetMessagesQueryResponse> GetMessagesAsync(GetMessagesQuery query);
    }
}
