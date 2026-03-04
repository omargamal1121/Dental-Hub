namespace DentalHub.Application.DTOs.Sessions
{
    /// DTO for session note
    public class SessionNoteDto
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
    }

    /// DTO for creating a session note
    public class CreateSessionNoteDto
    {
        public Guid SessionId { get; set; } 

        public string Note { get; set; } = string.Empty;
    }
}
