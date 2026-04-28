using DentalHub.Application.DTOs.Chat;

namespace DentalHub.Application.Interfaces
{
    public interface IChatService
    {
        ChatResponseDto ProcessNext(ChatRequestDto request);
    }
}
