namespace DentalHub.Application.DTOs.Identity
{
    public class RegisterStudentDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string University { get; set; } = string.Empty;

        public string UniversityId { get; set; } = null!;

        public string Username { get; set; } = null!;

        public int Level { get; set; }

        public string Phone { get; set; } = string.Empty;
    }
}
