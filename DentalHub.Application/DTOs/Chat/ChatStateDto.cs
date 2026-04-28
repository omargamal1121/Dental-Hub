using System.Text.Json.Serialization;

namespace DentalHub.Application.DTOs.Chat
{
    public class ChatStateDto
    {
        [JsonPropertyName("mode")]
        public string? Mode { get; set; }

        [JsonPropertyName("answers")]
        public Dictionary<string, string> Answers { get; set; } = new();

        [JsonPropertyName("asked")]
        public List<string> Asked { get; set; } = new();

        [JsonPropertyName("current_q")]
        public string? CurrentQ { get; set; }
    }
}
