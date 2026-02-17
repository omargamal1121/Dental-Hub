using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Session public ID is required")]
        public string SessionId { get; set; }

        [Required(ErrorMessage = "Note content is required")]
        [StringLength(2000, MinimumLength = 10)]
        public string Note { get; set; } = string.Empty;
    }
}
