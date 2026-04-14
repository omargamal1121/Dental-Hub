namespace DentalHub.Application.DTOs.Identity
{

    public class RegisterPatientDto
    {
        public string? UserName { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public Gender Gender { get; set; }

        public string NationalId { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public City City { get; set; }
    }
}
