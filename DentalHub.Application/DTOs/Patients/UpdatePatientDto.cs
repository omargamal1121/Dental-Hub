namespace DentalHub.Application.DTOs.Patients
{
    /// DTO for updating patient information
    public class UpdatePatientDto
    {
        public Guid PublicId { get; set; }

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public int? Age { get; set; }

        public string ? NationalId { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
	}
}
