namespace DentalHub.Application.DTOs.Admins
{
    public class RegisterAdminDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public bool IsSuperAdmin { get; set; } = false;
    }
}
