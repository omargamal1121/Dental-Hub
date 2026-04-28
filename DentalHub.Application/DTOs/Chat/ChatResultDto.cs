using System.Text.Json.Serialization;

namespace DentalHub.Application.DTOs.Chat
{
    public class ChatResultDto
    {
        [JsonPropertyName("diagnosis")]
        public string? Diagnosis { get; set; }
    }
}
