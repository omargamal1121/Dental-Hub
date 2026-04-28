using System.Text.Json.Serialization;

namespace DentalHub.Application.DTOs.Chat
{
    public class ChatResponseDto
    {
        [JsonPropertyName("done")]
        public bool Done { get; set; }

        [JsonPropertyName("question")]
        public string? Question { get; set; }

        [JsonPropertyName("result")]
        public ChatResultDto? Result { get; set; }

        [JsonPropertyName("state")]
        public ChatStateDto? State { get; set; }
    }
}
