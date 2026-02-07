using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Admins
{
    /// DTO for admin registration
    /// Data required when creating a new admin
    public class RegisterAdminDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [StringLength(100)]
        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; } = string.Empty;

        public bool IsSuperAdmin { get; set; } = false;
    }
}
