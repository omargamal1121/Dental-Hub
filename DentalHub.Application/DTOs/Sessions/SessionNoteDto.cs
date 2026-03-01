namespace DentalHub.Application.DTOs.Sessions
{
    /// DTO for session note
    public class SessionNoteDto
    {
        public string Id { get; set; }
        public string SessionId { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
    }

    /// DTO for creating a session note
    public class CreateSessionNoteDto
    {
        public string SessionId { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;
    }
}
