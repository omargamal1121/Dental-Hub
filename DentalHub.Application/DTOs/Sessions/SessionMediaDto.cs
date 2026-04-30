namespace DentalHub.Application.DTOs.Sessions
{
    /// DTO for session media (uploaded images)
    public class SessionMediaDto
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public Guid? NoteId { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
    }
}
