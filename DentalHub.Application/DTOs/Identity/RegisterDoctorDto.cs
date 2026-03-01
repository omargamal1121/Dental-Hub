namespace DentalHub.Application.DTOs.Identity
{
    /// DTO for doctor registration
    public class RegisterDoctorDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Specialty { get; set; } = string.Empty;

        public string UniversityId { get; set; } = string.Empty;

        public string Username { get; set; } = null!;

        public string Phone { get; set; } = null!;
    }
}
