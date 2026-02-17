namespace DentalHub.Application.DTOs.Admins
{
    /// DTO for admin information returned to frontend
    public class AdminDto
    {
        public string PublicId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsSuperAdmin { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
