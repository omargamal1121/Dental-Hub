using System.Text.Json.Serialization;

namespace DentalHub.Application.DTOs.Chat
{
    public class ChatRequestDto
    {
        [JsonPropertyName("answer")]
        public string? Answer { get; set; }

        [JsonPropertyName("state")]
        public ChatStateDto? State { get; set; }
    }
}
