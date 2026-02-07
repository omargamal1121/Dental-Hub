using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Admins
{
    /// DTO for updating admin information
    public class UpdateAdminDto
    {
        [Required]
        public Guid UserId { get; set; }

        [StringLength(100, MinimumLength = 3)]
        public string? FullName { get; set; }

        [StringLength(100)]
        public string? Role { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public bool? IsSuperAdmin { get; set; }
    }
}
