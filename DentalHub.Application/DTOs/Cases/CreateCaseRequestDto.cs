namespace DentalHub.Application.DTOs.Cases
{
    public class CreateCaseRequestDto
    {
        public Guid PatientCasePublicId { get; set; }

        public Guid StudentPublicId { get; set; }  // populated from JWT token, not the request body

        public string DoctorUsername { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
