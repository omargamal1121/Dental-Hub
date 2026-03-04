namespace DentalHub.Application.DTOs.Doctors
{
    /// DTO for updating doctor information
    public class UpdateDoctorDto
    {
        public Guid UserId { get; set; }

        public string? FullName { get; set; }

        public string? Name { get; set; }

        public string? Specialty { get; set; }

        public int UniversityId { get; set; }
    }
}
