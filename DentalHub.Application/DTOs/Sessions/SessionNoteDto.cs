using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Sessions
{
    /// <summary>
    /// DTO for session note
    /// </summary>
    public class SessionNoteDto
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a session note
    /// </summary>
    public class CreateSessionNoteDto
    {
        [Required]
        public Guid SessionId { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 10)]
        public string Note { get; set; } = string.Empty;
    }
}
