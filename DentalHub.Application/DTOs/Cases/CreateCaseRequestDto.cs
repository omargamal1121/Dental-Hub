namespace DentalHub.Application.DTOs.Cases
{
    public class CreateCaseRequestDto
    {
        public string PatientCasePublicId { get; set; } = string.Empty;

        public string StudentPublicId { get; set; } = string.Empty;

        public string DoctorPublicId { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
